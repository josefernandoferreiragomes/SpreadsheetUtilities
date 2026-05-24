using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public class GanttTaskProjectListGenerator : ListGenerator<GanttTask, Project>
{
    protected override string GetGroupKey(GanttTask item)
        => item?.ProjectName ?? "";

    protected override Project GenerateItem(string groupKey, IEnumerable<GanttTask> items)
        => new Project
        {
            ProjectID = items.First().ProjectID,
            ProjectName = groupKey,
            StartDate = items.Min(t => t.StartDate),
            EndDate = items.Max(t => t.EndDate),
            TotalEffortHours = items.Sum(t => t.EffortHours),
            ProjectGroup = _projectInputList.Find(p => p.ProjectName == groupKey)?.ProjectGroup
        };
}
