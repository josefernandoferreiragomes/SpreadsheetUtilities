using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Domain.Repositories;

public interface IHolidayRepository
{
    Task<List<Holiday>> GetAllAsync();
}
