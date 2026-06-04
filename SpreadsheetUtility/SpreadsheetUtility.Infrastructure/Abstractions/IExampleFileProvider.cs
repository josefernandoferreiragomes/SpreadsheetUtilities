using SpreadsheetUtility.Infrastructure.Models;

namespace SpreadsheetUtility.Infrastructure.Abstractions;

public interface IExampleFileProvider
{
    Task<IEnumerable<ExampleFileInfo>> GetAvailableFilesAsync();
    Task<FileDownloadDto> GetFileAsync(string fileName);
    Task<ExampleFileInfo?> GetFileInfoAsync(string fileName);
}
