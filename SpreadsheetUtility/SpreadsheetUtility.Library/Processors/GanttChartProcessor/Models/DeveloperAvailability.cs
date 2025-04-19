namespace SpreadsheetUtility.Library
{

    public class DeveloperAvailability
    {
        public string? Name { get; set; }
        public double DailyWorkHours { get; set; }
        public List<(DateTime Start, DateTime End)?>? VacationPeriods { get; set; }
        public DateTime NextAvailableDateForTasks { get; set; }
        public List<GanttTask>? Tasks { get; set; }
        public double AllocatedHours { get; set; }
        public double SlackHours { get; set; }
        public double TotalHours { get; set; }
        public string? VacationPeriodsSerialized { get; set; }
        
    }
}
