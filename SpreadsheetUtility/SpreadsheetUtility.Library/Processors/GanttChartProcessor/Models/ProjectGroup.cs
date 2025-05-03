namespace SpreadsheetUtility.Library.Models;

public class ProjectGroup
{
    public string ProjectGroupID { get; set; } = "";
    public List<Project> Projects { get; set; } = new List<Project>();
}
