namespace SpreadsheetUtility.Library.Models;

public class DeveloperAvailability
{
    public required string TeamId { get; set; }
    public required string Team { get; set; }
    public string? Name { get; set; }
    public required string DeveloperId { get; set; }
    public double DailyWorkHours { get; set; }
    public List<(DateTime Start, DateTime End)?>? VacationPeriods { get; set; }
    public DateTime NextAvailableDateForTasks { get; set; }
    public List<GanttTask>? Tasks { get; set; }
    public double AllocatedHours { get; set; }
    public double SlackHours { get; set; }
    public double TotalHours { get; set; }
    public string? VacationPeriodsSerialized { get; set; }
    public string? CustomClass { get; set; }
}

