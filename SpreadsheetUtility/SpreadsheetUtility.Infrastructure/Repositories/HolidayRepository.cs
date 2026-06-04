using SpreadsheetUtility.Domain.Models;
using SpreadsheetUtility.Domain.Repositories;
using SpreadsheetUtility.Infrastructure.Providers;

namespace SpreadsheetUtility.Infrastructure.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly HolidayFileProvider _holidayFileProvider;

    public HolidayRepository(HolidayFileProvider holidayFileProvider)
    {
        _holidayFileProvider = holidayFileProvider;
    }

    public Task<List<Holiday>> GetAllAsync()
    {
        var holidays = _holidayFileProvider.LoadHolidaysFromConfigurationFile();
        return Task.FromResult(holidays);
    }
}
