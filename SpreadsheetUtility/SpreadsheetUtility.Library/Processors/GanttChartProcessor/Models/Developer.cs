using SpreadsheetUtility.Library.Providers;

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
        
    private IDateTimeProvider _dateTimeProvider;
    public Developer(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        NextAvailableDateForTasks = _dateTimeProvider.Today;
    }

    public DateTime NextAvailableDate(DateTime fromDate)
    {
        DateTime date = fromDate > NextAvailableDateForTasks ? fromDate : NextAvailableDateForTasks;
        while (IsOnVacation(date) || IsWeekend(date))
        {
            date = date.AddDays(1);
        }
        return date;
    }
    private bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
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

