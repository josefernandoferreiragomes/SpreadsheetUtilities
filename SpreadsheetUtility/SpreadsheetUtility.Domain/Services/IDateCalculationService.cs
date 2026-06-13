using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Domain.Services;

public interface IDateCalculationService
{
    DateTime GetNextWorkingDay(DateTime dayDate);
    DateTime CalculateNextAvailableDate(Developer developer, DateTime fromDate);
}
