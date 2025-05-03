namespace SpreadsheetUtility.Library.Models;

public class Project
{
    public string? ProjectID { get; set; }
    public string? ProjectName { get; set; }        
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<GanttTask>? Tasks { get; set; }
    public double TotalEstimatedEffortHours { get; set; }
    public string? ProjectGroup { get; set; }
    public string? TeamId { get; set; }
    public string? Color { get; set; }
}

