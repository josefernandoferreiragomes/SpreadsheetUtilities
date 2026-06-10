using SpreadsheetUtility.Domain.Models;
using SpreadsheetUtility.Domain.ValueObjects;

namespace SpreadsheetUtility.Domain.Services;

public interface IHolidayLookupService
{
    bool IsHoliday(DateTime date);
    List<Holiday> GetHolidaysBetween(DateRange range);
}
