using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Services;

public interface IDateCalculator
{
    DateTime GetNextWorkingDay(DateTime startDate);
    DateTime CalculateEndDate(DateTime startDate, double workDays, List<(DateTime Start, DateTime End)?>? vacations);
    int CalculateIntervalDays(DateTime startDate, DateTime endDate, List<(DateTime Start, DateTime End)?>? vacations);
    int CalculateWorkDays(DateTime startDate, DateTime endDate, List<(DateTime Start, DateTime End)?>? vacations);
    int CalculateVacationDays(DateTime startDate, DateTime endDate, List<(DateTime Start, DateTime End)?>? vacations);
    int CalculateNonWorkingDays(DateTime startDate, DateTime endDate, List<(DateTime Start, DateTime End)?>? vacations);
    void AddObserver(IObserver<Holiday> observer);
    void RemoveObserver(IObserver<Holiday> observer);
}
