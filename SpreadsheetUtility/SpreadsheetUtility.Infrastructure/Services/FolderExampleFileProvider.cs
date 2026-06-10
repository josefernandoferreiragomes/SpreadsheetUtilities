using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SpreadsheetUtility.Infrastructure.Abstractions;
using SpreadsheetUtility.Infrastructure.Models;

namespace SpreadsheetUtility.Infrastructure.Services;

public partial class FolderExampleFileProvider : IExampleFileProvider
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

    public async Task<IEnumerable<ExampleFileInfo>> GetAvailableFilesAsync()
    {
        try
        {
            if (!Directory.Exists(_exampleFilesPath))
            {
                Log.DirectoryNotFound(_logger, _exampleFilesPath);
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
            Log.ListError(_logger, _exampleFilesPath, ex);
            throw;
        }
    }

    public async Task<FileDownloadDto> GetFileAsync(string fileName)
    {
        ValidateFileName(fileName);

        try
        {
            var filePath = Path.Combine(_exampleFilesPath, fileName);

            if (!File.Exists(filePath))
            {
                Log.FileNotFound(_logger, fileName);
                throw new FileNotFoundException($"File not found: {fileName}");
            }

            var fileInfo = new FileInfo(filePath);
            var content = await File.ReadAllBytesAsync(filePath);

            Log.FileRetrieved(_logger, fileName, fileInfo.Length);

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
            Log.ReadError(_logger, fileName, ex);
            throw;
        }
    }

    public async Task<ExampleFileInfo?> GetFileInfoAsync(string fileName)
    {
        ValidateFileName(fileName);

        try
        {
            var filePath = Path.Combine(_exampleFilesPath, fileName);

            if (!File.Exists(filePath))
            {
                Log.FileInfoNotFound(_logger, fileName);
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
            Log.InfoError(_logger, fileName, ex);
            return null;
        }
    }

    private void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
            throw new ArgumentException("Invalid file name - directory traversal detected", nameof(fileName));

        if (!fileName.EndsWith(AllowedExtension, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Only {AllowedExtension} files are allowed", nameof(fileName));

        if (fileName.IndexOf('\0') >= 0)
            throw new ArgumentException("Invalid file name - contains null characters", nameof(fileName));
    }

    private string GenerateDescription(string fileName)
    {
        return fileName switch
        {
            _ => "Example spreadsheet for Gantt chart generation"
        };
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Example files directory not found: {Path}")]
        public static partial void DirectoryNotFound(ILogger logger, string path);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error retrieving available files from {Path}")]
        public static partial void ListError(ILogger logger, string path, Exception exception);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Example file not found: {FileName}")]
        public static partial void FileNotFound(ILogger logger, string fileName);

        [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Retrieved example file: {FileName} ({SizeBytes} bytes)")]
        public static partial void FileRetrieved(ILogger logger, string fileName, long sizeBytes);

        [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Error reading file: {FileName}")]
        public static partial void ReadError(ILogger logger, string fileName, Exception exception);

        [LoggerMessage(EventId = 6, Level = LogLevel.Debug, Message = "Example file not found: {FileName}")]
        public static partial void FileInfoNotFound(ILogger logger, string fileName);

        [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "Error getting file info: {FileName}")]
        public static partial void InfoError(ILogger logger, string fileName, Exception exception);
    }
}