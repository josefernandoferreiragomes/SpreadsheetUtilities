using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public class GroupProjectsByProjectGroupQuery
{
#pragma warning disable CA1822 // Mark members as static
    public List<ProjectGroup> Execute(List<Project> projects)
#pragma warning restore CA1822 // Mark members as static
    => projects.GroupBy(p => p.ProjectGroup)
        .Select(g => new ProjectGroup
        {
            ProjectGroupID = g.Key ?? "",
            Projects = g.ToList()
        }).ToList();
}
