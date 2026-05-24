using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public class DeveloperTaskListGenerator : ListGenerator<Developer, List<GanttTask>>
{
    protected override string GetGroupKey(Developer item)
        => item.Name;

    protected override List<GanttTask> GenerateItem(string groupKey, IEnumerable<Developer> items)
    {
        var developer = items.First();

        var ganttTasks = new List<GanttTask>();

        foreach (var task in developer.Tasks)
        {
            ganttTasks.Add(new GanttTask
            {
                Id = task.Id,
                TaskName = task.TaskName,
                Start = task.Start,
                End = task.End,
                Progress = task.Progress,
                Dependencies = task.Dependencies,
                AssignedDeveloper = task.AssignedDeveloper,
                AssignedDeveloperId = task.AssignedDeveloperId,
                CustomClass = "task",
                Resource = developer.Name,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                ProjectName = task.ProjectName,
                ProjectID = task.ProjectID,
                TaskExtendedDescription = task.TaskExtendedDescription,
                InternalID = task.InternalID
            });
        }

        int vacationId = 1;
        foreach (var vacation in developer.VacationPeriods ?? Enumerable.Empty<(DateTime Start, DateTime End)?>())
        {
            if (!vacation.HasValue || vacation.Value.End < _projectStartDate) continue;

            ganttTasks.Add(new GanttTask
            {
                Id = $"vacation-{developer.Name}-{vacationId}",
                TaskName = $"{developer.Name} - Vacation {vacationId}",
                Start = vacation.Value.Start.ToString("yyyy-MM-dd"),
                End = vacation.Value.End.ToString("yyyy-MM-dd"),
                CustomClass = "task",
                Resource = developer.Name,
                StartDate = vacation.Value.Start,
                EndDate = vacation.Value.End,
                ProjectName = "Vacation",
                ProjectID = "VacationID",
                TaskExtendedDescription = "Vacation"
            });

            vacationId++;
        }

        return ganttTasks;
    }
}
