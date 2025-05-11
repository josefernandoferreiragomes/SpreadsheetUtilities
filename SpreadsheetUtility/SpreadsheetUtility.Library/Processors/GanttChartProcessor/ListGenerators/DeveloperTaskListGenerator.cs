using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.ListGenerators;
public class DeveloperTaskListGenerator : ListGenerator<Developer, List<GanttTask>>
{
    protected override string GetGroupKey(Developer item)
        => item.Name; // or item.ID if more appropriate    

    protected override List<GanttTask> GenerateItem(string groupKey, IEnumerable<Developer> items)
    {
        var developer = items.First(); // only one per groupKey due to grouping

        var ganttTasks = new List<GanttTask>();

        // Developer tasks
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

        // Vacation tasks
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

