using SpreadsheetUtility.Library.Models;

namespace SpreadsheetUtility.Library.Calculators;
public interface IDateCalculator
{
    DateTime GetNextWorkingDay(DateTime startDate);
    DateTime CalculateEndDate(DateTime start, double workDays, List<(DateTime Start, DateTime End)?>? vacations);
    int CalculateIntervalDays(DateTime start, DateTime end, List<(DateTime Start, DateTime End)?>? vacations);
    void AddObserver(IObserver<Holiday> observer);
    void RemoveObserver(IObserver<Holiday> observer);
}