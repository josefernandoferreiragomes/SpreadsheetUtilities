using SpreadsheetUtility.Library.Calculators;

namespace SpreadsheetUtility.Library.Models;

public class Developer
{
    public required string TeamId { get; set; }
    public required string Team { get; set; }
    public required string Name { get; set; }
    public required string DeveloperId { get; set; }
    public double DailyWorkHours { get; set; }
    public double AllocatedDays { get; set; }
    public List<(DateTime Start, DateTime End)?>? VacationPeriods { get; set; }
    public DateTime NextAvailableDateForTasks { get; set; }
    public List<GanttTask> Tasks { get; set; } = new List<GanttTask>();
    public double AllocatedHours { get; set; }
    public double SlackHours { get; set; }
    public double TotalHours { get; set; }
    public string VacationPeriodsSerialized => string.Join("|", VacationPeriods?.Where(v => v.HasValue)
        .Select(v => $"{v?.Start:yyyy-MM-dd};{v?.End:yyyy-MM-dd}") ?? Enumerable.Empty<string>());

    private readonly IDateCalculator _dateCalculator;

    public Developer(IDateCalculator dateCalculator)
    {
        _dateCalculator = dateCalculator;
        NextAvailableDateForTasks = _dateCalculator.GetNextWorkingDay(DateTime.Today);
    }

    public DateTime NextAvailableDate(DateTime fromDate)
    {
        // Use DateCalculator to get the next working day, considering vacations        
        DateTime date = _dateCalculator.GetNextWorkingDay(fromDate > NextAvailableDateForTasks ? fromDate : NextAvailableDateForTasks);
        while (IsOnVacation(date))
        {
            date = date.AddDays(1);
        }
        return date;
    }
    private bool IsOnVacation(DateTime date)
    {
        return VacationPeriods?.Any(v => v.HasValue && date >= v.Value.Start && date <= v.Value.End) ?? false;
    }
    public void SetNextAvailableDate(DateTime date)
    {
        NextAvailableDateForTasks = date;
    }
}
