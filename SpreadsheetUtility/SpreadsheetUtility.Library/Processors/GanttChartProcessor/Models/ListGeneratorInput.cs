namespace SpreadsheetUtility.Library.Models;

public class ListGeneratorInput
{
    public List<Project> projectInputList { get; set; } = new List<Project>();
    public DateTime projectStartDate { get; set; }
}

