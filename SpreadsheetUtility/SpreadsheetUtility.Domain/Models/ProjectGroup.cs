namespace SpreadsheetUtility.Domain.Models;

public class ProjectGroup
{
    public string ProjectGroupID { get; set; } = "";
    public List<Project> Projects { get; set; } = new List<Project>();
}
