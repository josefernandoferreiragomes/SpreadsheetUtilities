using SpreadsheetUtility.Library.Calculators;
using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Processors;

namespace SpreadsheetUtility.Library.TaskAssigners;
public abstract class TaskAssignmentStrategyBase(IDateCalculator _dateCalculator) : ITaskAssignmentStrategy
{
    protected IDateCalculator _dateCalculator = _dateCalculator;
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
            var assignedDeveloper = developerList.OrderBy(d => d.NextAvailableDate(startDate)).FirstOrDefault();
            if (assignedDeveloper == null) continue;

            DateTime taskStart = assignedDeveloper.NextAvailableDate(startDate);
            var taskStartFromDependency = projectTasks.Find(t => t.Id == task.Dependencies)?.EndDate ?? DateTime.MinValue;

            taskStart = taskStart > taskStartFromDependency ? taskStart : _dateCalculator.GetNextWorkingDay(taskStartFromDependency.AddDays(1));

            double requiredDays = Math.Ceiling(task.EstimatedEffortHours / assignedDeveloper.DailyWorkHours);
            DateTime taskEnd = _dateCalculator.CalculateEndDate(taskStart, requiredDays, assignedDeveloper.VacationPeriods);

            task.Start = taskStart.ToString("yyyy-MM-dd");
            task.End = taskEnd.ToString("yyyy-MM-dd");
            //TaskEndWeekDescription is equal to the first day of the week of the task end date
            task.TaskEndWeekDescription = $"Week of {taskEnd.AddDays(-(int)(taskEnd.DayOfWeek - 1)).ToString("yyyy-MM-dd")}";
            task.StartDate = taskStart;
            task.EndDate = taskEnd;
            task.AssignedDeveloper = assignedDeveloper.Name;
            task.AssignedDeveloperId = assignedDeveloper.DeveloperId;
            task.Name = $"{task.Name} ({assignedDeveloper.Name})";
            task.DailyWorkHours = (int?)assignedDeveloper.DailyWorkHours;
            task.IntervalDays = _dateCalculator.CalculateIntervalDays(taskStart, taskEnd, assignedDeveloper.VacationPeriods);
            task.WorkDays = _dateCalculator.CalculateWorkDays(taskStart, taskEnd, assignedDeveloper.VacationPeriods);
            task.VacationDays = _dateCalculator.CalculateVacationDays(taskStart, taskEnd, assignedDeveloper.VacationPeriods);
            task.NonWorkingDays = _dateCalculator.CalculateNonWorkingDays(taskStart, taskEnd, assignedDeveloper.VacationPeriods);
            assignedDeveloper.Tasks.Add(task);

            assignedDeveloper.SetNextAvailableDate(_dateCalculator.GetNextWorkingDay(taskEnd.AddDays(1)));
        }
    }
}