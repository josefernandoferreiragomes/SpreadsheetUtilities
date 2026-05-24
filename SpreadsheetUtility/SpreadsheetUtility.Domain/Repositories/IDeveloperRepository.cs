using SpreadsheetUtility.Domain.Models;

namespace SpreadsheetUtility.Domain.Repositories;

public interface IDeveloperRepository
{
    Task<List<Developer>> GetAllAsync();
}
