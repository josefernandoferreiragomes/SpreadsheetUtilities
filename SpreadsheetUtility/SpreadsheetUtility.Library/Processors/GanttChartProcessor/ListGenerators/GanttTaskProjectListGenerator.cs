using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.ListGenerators;
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
                //TODO: verify if it is necessary to include tasks in the project object
                //Tasks = items.ToList(),
                TotalEffortHours = items.Sum(t => t.EffortHours),
                ProjectGroup = _projectInputList.Find(p => p.ProjectName == groupKey)?.ProjectGroup
            };    
}