using SpreadsheetUtility.Application.DTOs;

namespace SpreadsheetUtility.Application.Ports;

public interface IDoubleEntryGeneratorService
{
    Task<GenerateDoubleEntryOutput> GenerateAsync(GenerateDoubleEntryInput input, CancellationToken cancellationToken = default);
}
