using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Infrastructure.Providers;

public class HolidayFileProvider : Application.Ports.IHolidayProvider
{
    private readonly ILogger<List<Holiday>> _logger;
    private readonly IConfiguration _configuration;
    public HolidayFileProvider(ILogger<List<Holiday>> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    public List<Holiday> LoadHolidaysFromConfigurationFile()
    {
        var holidayList = new List<Holiday>();
        var filePath = Path.Combine(AppContext.BaseDirectory, _configuration.GetSection("HolidaysFile")?.Value ?? "");

        try
        {
            var jsonString = File.ReadAllText(filePath);
            holidayList = JsonConvert.DeserializeObject<List<Holiday>>(jsonString) ?? new List<Holiday>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred processing holidays: {ex.Message}");
        }

        return holidayList;
    }
}
