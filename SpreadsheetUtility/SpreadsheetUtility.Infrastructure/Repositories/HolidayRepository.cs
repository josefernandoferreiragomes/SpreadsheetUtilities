using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Domain.Models;
using SpreadsheetUtility.Domain.Repositories;

namespace SpreadsheetUtility.Infrastructure.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly IHolidayProvider _holidayProvider;

    public HolidayRepository(IHolidayProvider holidayProvider)
    {
        _holidayProvider = holidayProvider;
    }

    public Task<List<Holiday>> GetAllAsync()
    {
        var holidays = _holidayProvider.LoadHolidaysFromConfigurationFile();
        return Task.FromResult(holidays);
    }
}
