using SpreadsheetUtility.Domain.Models;
using SpreadsheetUtility.Domain.Repositories;

namespace SpreadsheetUtility.Infrastructure.Repositories;

public class DeveloperRepository : IDeveloperRepository
{
    public Task<List<Developer>> GetAllAsync()
    {
        return Task.FromResult(new List<Developer>());
    }
}
