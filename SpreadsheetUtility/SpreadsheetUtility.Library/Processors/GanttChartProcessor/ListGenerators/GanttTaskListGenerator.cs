using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.ListGenerators;
public class GanttTaskListGenerator : ListGenerator<GanttTask, GanttTask>
{
    protected override string GetGroupKey(GanttTask item) => item?.ProjectName ?? "";

    protected override GanttTask GenerateItem(string groupKey, IEnumerable<GanttTask> items)
    {
        double totalEffortHours = items.Sum(t => t.EffortHours);

        return new GanttTask
        {         
            Id = items.First().ProjectID ?? "",
            TaskName = $"{groupKey} ({items.Sum(t => t.EffortHours)} hours)",
            Dependencies = items.Select(t => t.ProjectDependency).Where(d => !string.IsNullOrEmpty(d)).FirstOrDefault() ?? string.Empty,
            EffortHours = items.Sum(t => t.EffortHours),
            Progress = (int)(items.Sum(t => (t.Progress * (t.EffortHours / totalEffortHours)))), // Summing the progress of each related task
            StartDate = items.Min(t => t.StartDate),
            EndDate = items.Max(t => t.EndDate),
            Start = items.Min(t => t.StartDate).ToString("yyyy-MM-dd"),
            End = items.Max(t => t.EndDate).ToString("yyyy-MM-dd"),
            ProjectName = groupKey,
            ProjectID = items.First().ProjectID ?? "",
        };
    }
}
