using SpreadsheetUtility.Domain.Models;
using SpreadsheetUtility.Domain.ValueObjects;

namespace SpreadsheetUtility.Domain.Services;

public interface IHolidayLookupService
{
    bool IsHoliday(DateTime dayDate);
    List<Holiday> GetHolidaysBetween(DateRange range);
}
