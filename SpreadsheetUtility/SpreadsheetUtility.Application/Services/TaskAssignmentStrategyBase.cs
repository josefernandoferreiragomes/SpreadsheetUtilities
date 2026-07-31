using SpreadsheetUtility.Domain.Models;
using System.Globalization;

namespace SpreadsheetUtility.Application.Services;

public abstract class TaskAssignmentStrategyBase(IDateCalculator dateCalculator) : ITaskAssignmentStrategy
{   
    internal readonly IDateCalculator _dateCalculator = dateCalculator;

    public abstract void AssignTasks(
        ProjectGroup projectGroup,
        List<Developer> developers,
        List<GanttTask> tasks,
        DateTime projectStartDate
    );

    protected void AssignTasksToDevelopers(List<Developer> developerList, List<GanttTask> projectTasks, DateTime startDate)
    {
        foreach (var task in projectTasks)
        {
            var assignedDeveloper = SelectDeveloperForTask(developerList, startDate);
            if (assignedDeveloper == null) continue;

            var (taskStart, taskEnd) = ScheduleTask(task, assignedDeveloper, startDate, _dateCalculator, projectTasks);

            UpdateTaskDetails(task, assignedDeveloper, taskStart, taskEnd, _dateCalculator);

            assignedDeveloper.Tasks.Add(task);
            assignedDeveloper.SetNextAvailableDate(_dateCalculator.GetNextWorkingDay(taskEnd.AddDays(1)));
        }
    }

    private Developer? SelectDeveloperForTask(List<Developer> developerList, DateTime startDate)
        => developerList.OrderBy(d => GetNextAvailableDateForDeveloper(d, startDate)).FirstOrDefault();

    private DateTime GetNextAvailableDateForDeveloper(Developer developer, DateTime fromDate)
    {
        DateTime date = _dateCalculator.GetNextWorkingDay(fromDate > developer.NextAvailableDateForTasks ? fromDate : developer.NextAvailableDateForTasks);
        while (developer.IsOnVacation(date))
        {
            date = date.AddDays(1);
        }
        return date;
    }

    private (DateTime TaskStart, DateTime TaskEnd) ScheduleTask(
        GanttTask task,
        Developer developer,
        DateTime startDate,
        IDateCalculator dateCalculator,
        List<GanttTask> projectTasks
    )
    {
        DateTime taskStart = GetNextAvailableDateForDeveloper(developer, startDate);
        var taskStartFromDependency = projectTasks.Find(t => t.Id == task.Dependencies)?.EndDate ?? DateTime.MinValue;

        taskStart = taskStart > taskStartFromDependency
            ? taskStart
            : dateCalculator.GetNextWorkingDay(taskStartFromDependency.AddDays(1));

        double requiredDays = Math.Ceiling(task.EffortHours / developer.DailyWorkHours);
        DateTime taskEnd = dateCalculator.CalculateEndDate(taskStart, requiredDays, developer.VacationPeriods);

        return (taskStart, taskEnd);
    }

    private static void UpdateTaskDetails(
        GanttTask task,
        Developer developer,
        DateTime taskStart,
        DateTime taskEnd,
        IDateCalculator dateCalculator
    )
    {
        task.Start = taskStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        task.End = taskEnd.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        task.TaskEndWeekDescriptionDescription = $"Week of {taskEnd.AddDays(-(int)(taskEnd.DayOfWeek - 1)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
        task.StartDate = taskStart;
        task.EndDate = taskEnd;
        task.AssignedDeveloper = developer.Name;
        task.AssignedDeveloperId = developer.DeveloperId;
        task.TaskName = $"{task.TaskName} ({developer.Name})";
        task.DailyWorkHours = (int?)developer.DailyWorkHours;
        task.IntervalDays = dateCalculator.CalculateIntervalDays(taskStart, taskEnd, developer.VacationPeriods);
        task.WorkDays = dateCalculator.CalculateWorkDays(taskStart, taskEnd, developer.VacationPeriods);
        task.VacationDays = dateCalculator.CalculateVacationDays(taskStart, taskEnd, developer.VacationPeriods);
        task.NonWorkingDays = dateCalculator.CalculateNonWorkingDays(taskStart, taskEnd, developer.VacationPeriods);
    }
}
