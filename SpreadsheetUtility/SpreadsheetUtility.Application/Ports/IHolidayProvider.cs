using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Application.Ports;

public interface IHolidayProvider
{
    List<Holiday> LoadHolidaysFromConfigurationFile();
}
