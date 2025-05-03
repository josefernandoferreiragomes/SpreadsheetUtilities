namespace SpreadsheetUtility.Library.Models;

public class DeveloperDto
{
     
    public required string TeamId { get; set; }
    public required string Team { get; set; }
    public required string DeveloperId { get; set; }
    public required string Name { get; set; }
    public required double DailyWorkHours { get; set; }
    public required string VacationPeriods { get; set; }
}

