using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.Grouppers;
public class GroupProjectsByProjectGroupQuery
{
    public List<ProjectGroup> Execute(List<Project> projects)
    => projects.GroupBy(p => p.ProjectGroup)
        .Select(g => new ProjectGroup
        {
            ProjectGroupID = g.Key ?? "",
            Projects = g.ToList()
        }).ToList();
}
