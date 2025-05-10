using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Providers;

namespace SpreadsheetUtility.Library.Calculators
{    

    public class DateCalculator : IDateCalculator
    {
        private readonly List<IObserver<Holiday>> _observers = new();
        private readonly IHolidayProvider _holidayProvider;
        private List<Holiday> _holidayList;
        private List<Holiday> _projectHolidayList;

        public DateCalculator(IHolidayProvider holidayProvider)
        {
            _holidayProvider = holidayProvider;
            _holidayList = _holidayProvider.LoadHolidaysFromConfigurationFile();
            _projectHolidayList = new List<Holiday>();
            
        }
        
        public void AddObserver(IObserver<Holiday> observer)
        {
            _observers.Add(observer);
        }        
        public void RemoveObserver(IObserver<Holiday> observer)
        {
            _observers.Remove(observer);
        }
        private void NotifyObservers(Holiday holiday)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(holiday);
            }
        }       

        public DateTime GetNextWorkingDay(DateTime startDate)
        {
            while (IsWeekendOrHoliday(startDate))
            {
                startDate = startDate.AddDays(1);
            }

            return startDate;
        }

        public DateTime CalculateEndDate(DateTime start, double workDays, List<(DateTime Start, DateTime End)?>? vacations)
        {
            DateTime end = start;
            while (workDays > 0)
            {
                if (!IsVacationDay(end, vacations) && !IsWeekendOrHoliday(end)) workDays--;
                end = end.AddDays(1);
            }
            return end.AddDays(-1);
        }

        public int CalculateIntervalDays(DateTime start, DateTime end, List<(DateTime Start, DateTime End)?>? vacations)
        {
            int days = 1;
            var startDate = start;
            while (startDate < end)
            {                
                days++;               
                startDate = startDate.AddDays(1);
            }
            return days;
        }

        public int CalculateWorkDays(DateTime start, DateTime end, List<(DateTime Start, DateTime End)?>? vacations)
        {
            int days = 1;
            var startDate = start;
            while (startDate < end)
            {
                if (!IsVacationDay(startDate, vacations) && !IsWeekendOrHoliday(startDate))
                {
                    days++;
                }
                startDate = startDate.AddDays(1);
            }
            return days;
        }

        public int CalculateVacationDays(DateTime start, DateTime end, List<(DateTime Start, DateTime End)?>? vacations)
        {
            int days = 1;
            var startDate = start;
            while (startDate < end)
            {
                if (IsVacationDay(startDate, vacations))
                {
                    days++;
                }
                startDate = startDate.AddDays(1);
            }
            return days;
        }

        public int CalculateNonWorkingDays(DateTime start, DateTime end, List<(DateTime Start, DateTime End)?>? vacations)
        {
            int days = 1;
            var startDate = start;
            while (startDate < end)
            {
                if (IsVacationDay(startDate, vacations) || IsWeekendOrHoliday(startDate))
                {
                    days++;
                }
                startDate = startDate.AddDays(1);
            }
            return days;
        }
        private bool IsHoliday(DateTime date)
        {
            bool isHoliday = false;
            isHoliday = _holidayList.Select(h => h.Date).Contains(date);
            if (isHoliday)
            {
                if (!_projectHolidayList.Select(h => h.Date).Contains(date))                    
                    NotifyObservers(_holidayList.Find(h => h.Date == date) ?? new Holiday());
            }
            return isHoliday;
        }        

        private bool IsVacationDay(DateTime date, List<(DateTime Start, DateTime End)?>? vacations)
        {
            return vacations?.Any(v => v.HasValue && date >= v.Value.Start && date <= v.Value.End) ?? false;
        }

        private bool IsWeekendOrHoliday(DateTime date)
        {
            return IsWeekend(date) || IsHoliday(date);
        }
        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
