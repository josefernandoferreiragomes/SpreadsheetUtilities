using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.ListGenerators;
public class DeveloperTaskListGenerator : ListGenerator<Developer, GanttTask>
{
    protected override string GetGroupKey(Developer developer) => developer.DeveloperId;

    protected override GanttTask GenerateItem(string groupKey, IEnumerable<Developer> developers)
    {
        var developer = developers.First(); // Since developers are grouped by DeveloperId, this will always be a single developer.

        var ganttTasks = new List<GanttTask>();

        // Add tasks assigned to the developer
        foreach (var task in developer.Tasks)
        {
            ganttTasks.Add(new GanttTask
            {
                Id = task.Id,
                Name = task.Name,
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
                TaskName = task.TaskName,
                InternalID = task.InternalID
            });
        }

        // Add vacation periods as separate tasks
        int vacationId = 1;
        foreach (var vacation in developer.VacationPeriods ?? new List<(DateTime Start, DateTime End)?>())
        {
            if (!vacation.HasValue || vacation.Value.Start < _projectStartDate) continue;
            var start = vacation.Value.Start;
            var end = vacation.Value.End;
            ganttTasks.Add(new GanttTask
            {
                Id = vacationId.ToString(),
                Name = $"{developer.Name} - Vacation {vacationId}",
                Start = start.ToString("yyyy-MM-dd"),
                End = end.ToString("yyyy-MM-dd"),
                CustomClass = "task",
                Resource = developer.Name,
                StartDate = start,
                EndDate = end,
                ProjectName = "Vacation",
                ProjectID = "VacationID",
                TaskName = "Vacation",
            });
            vacationId++;
        }

        // Return the combined list of tasks
        return ganttTasks.First(); // Adjust this logic if you need to return a single GanttTask or a collection.
    }
}
