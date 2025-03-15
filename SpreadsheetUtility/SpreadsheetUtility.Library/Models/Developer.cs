using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtility.Library
{
    
    public class Developer
    {
        public required string Name { get; set; }
        public double DailyWorkHours { get; set; }
        public List<(DateTime Start, DateTime End)?>? VacationPeriods { get; set; }

        public DateTime NextAvailableDateForTasks = DateTime.Today;
        public List<GanttTask> Tasks { get; set; } = new List<GanttTask>();
        public double AllocatedHours { get; set; }
        public double SlackHours { get; set; }
        public double TotalHours { get; set; }
        public string VacationPeriodsSerialized => string.Join("|", VacationPeriods?.Where(v => v.HasValue)
            .Select(v => $"{v?.Start:yyyy-MM-dd};{v?.End:yyyy-MM-dd}") ?? Enumerable.Empty<string>());
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
}
