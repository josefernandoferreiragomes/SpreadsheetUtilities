namespace SpreadsheetUtility.UI.Web.Services;

using SpreadsheetUtility.UI.Web.Models;

/// <summary>
/// Serves example files from the local application folder (wwwroot/ExampleFiles).
/// This is the current implementation and works well for single-server deployments.
/// When scaling to multiple servers, migrate to FileServerExampleFileProvider.
/// </summary>
public class FolderExampleFileProvider : IExampleFileProvider
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ILogger<FolderExampleFileProvider> _logger;
    private readonly string _exampleFilesPath;

    private const string ExampleFilesFolder = "ExampleFiles";
    private const string AllowedExtension = ".xlsx";

    public FolderExampleFileProvider(IWebHostEnvironment hostEnvironment, ILogger<FolderExampleFileProvider> logger)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
        _exampleFilesPath = Path.Combine(_hostEnvironment.WebRootPath, ExampleFilesFolder);
    }

    /// <summary>
    /// Gets all available example files from the ExampleFiles folder.
    /// </summary>
    public async Task<IEnumerable<ExampleFileInfo>> GetAvailableFilesAsync()
    {
        try
        {
            if (!Directory.Exists(_exampleFilesPath))
            {
                _logger.LogWarning("Example files directory not found: {Path}", _exampleFilesPath);
                return Enumerable.Empty<ExampleFileInfo>();
            }

            var directory = new DirectoryInfo(_exampleFilesPath);
            var files = directory.GetFiles($"*{AllowedExtension}")
                .Select(f => new ExampleFileInfo
                {
                    FileName = f.Name,
                    DisplayName = Path.GetFileNameWithoutExtension(f.Name),
                    Description = GenerateDescription(f.Name),
                    FileSizeBytes = f.Length,
                    LastModified = f.LastWriteTime
                })
                .OrderBy(f => f.DisplayName)
                .ToList();

            return await Task.FromResult(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available files from {Path}", _exampleFilesPath);
            throw;
        }
    }

    /// <summary>
    /// Gets a specific example file including its content.
    /// </summary>
    public async Task<FileDownloadDto> GetFileAsync(string fileName)
    {
        ValidateFileName(fileName);

        try
        {
            var filePath = Path.Combine(_exampleFilesPath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Example file not found: {FileName}", fileName);
                throw new FileNotFoundException($"File not found: {fileName}");
            }

            var fileInfo = new FileInfo(filePath);
            var content = await File.ReadAllBytesAsync(filePath);

            _logger.LogInformation("Retrieved example file: {FileName} ({SizeBytes} bytes)", fileName, fileInfo.Length);

            return new FileDownloadDto
            {
                FileName = fileName,
                DisplayName = Path.GetFileNameWithoutExtension(fileName),
                Content = content,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FileName}", fileName);
            throw;
        }
    }

    /// <summary>
    /// Gets file information without loading the content.
    /// </summary>
    public async Task<ExampleFileInfo?> GetFileInfoAsync(string fileName)
    {
        ValidateFileName(fileName);

        try
        {
            var filePath = Path.Combine(_exampleFilesPath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogDebug("Example file not found: {FileName}", fileName);
                return null;
            }

            var fileInfo = new FileInfo(filePath);
            return await Task.FromResult(new ExampleFileInfo
            {
                FileName = fileName,
                DisplayName = Path.GetFileNameWithoutExtension(fileName),
                Description = GenerateDescription(fileName),
                FileSizeBytes = fileInfo.Length,
                LastModified = fileInfo.LastWriteTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file info: {FileName}", fileName);
            return null;
        }
    }

    /// <summary>
    /// Validates the file name to prevent directory traversal attacks.
    /// </summary>
    private void ValidateFileName(string fileName)
    {
        // Prevent directory traversal attacks
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
            throw new ArgumentException("Invalid file name - directory traversal detected", nameof(fileName));

        if (!fileName.EndsWith(AllowedExtension, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Only {AllowedExtension} files are allowed", nameof(fileName));

        // Additional validation: ensure no null characters
        if (fileName.IndexOf('\0') >= 0)
            throw new ArgumentException("Invalid file name - contains null characters", nameof(fileName));
    }

    /// <summary>
    /// Generates a human-readable description based on the file name.
    /// Can be extended to read descriptions from a configuration file or database.
    /// </summary>
    private string GenerateDescription(string fileName)
    {
        // In the future, this could read from a metadata file or database
        return fileName switch
        {
            _ => "Example spreadsheet for Gantt chart generation"
        };
    }
}
