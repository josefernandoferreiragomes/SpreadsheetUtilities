using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Domain.Services;

public interface IDateCalculationService
{
    DateTime GetNextWorkingDay(DateTime date);
    DateTime CalculateNextAvailableDate(Developer developer, DateTime fromDate);
}
