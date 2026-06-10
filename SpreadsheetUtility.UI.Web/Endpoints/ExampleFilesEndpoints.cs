using Microsoft.Extensions.Logging;
using SpreadsheetUtility.Infrastructure.Abstractions;
using SpreadsheetUtility.Infrastructure.Models;

namespace SpreadsheetUtility.UI.Web.Endpoints;

public static partial class ExampleFilesEndpoints
{
    public static IEndpointRouteBuilder MapExampleFilesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/examplefiles")
            .WithTags("ExampleFiles");

        // GET /api/examplefiles — list all available files
        group.MapGet("/", async (IExampleFileProvider fileProvider, ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("ExampleFiles");
            try
            {
                Log.ListRequested(logger);
                var files = await fileProvider.GetAvailableFilesAsync();
                return Results.Ok(files);
            }
            catch (Exception ex)
            {
                Log.ListError(logger, ex);
                return Results.Problem(detail: ex.Message, statusCode: 500, title: "Error retrieving files");
            }
        })
        .Produces<IEnumerable<ExampleFileInfo>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/examplefiles/{fileName} — download a file
        group.MapGet("/{fileName}", async (string fileName, IExampleFileProvider fileProvider, ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("ExampleFiles");
            try
            {
                Log.DownloadRequested(logger, fileName);
                var fileData = await fileProvider.GetFileAsync(fileName);
                return Results.File(fileData.Content, fileData.ContentType, fileData.FileName);
            }
            catch (FileNotFoundException ex)
            {
                Log.DownloadFileNotFound(logger, fileName, ex);
                return Results.NotFound(new { message = $"File not found: {fileName}" });
            }
            catch (ArgumentException ex)
            {
                Log.DownloadInvalidFileName(logger, fileName, ex);
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.DownloadError(logger, fileName, ex);
                return Results.Problem(detail: ex.Message, statusCode: 500, title: "Error downloading file");
            }
        });

        // HEAD /api/examplefiles/{fileName} — get file info
        group.MapMethods("/{fileName}", new[] { "HEAD" }, async (string fileName, IExampleFileProvider fileProvider, ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("ExampleFiles");
            try
            {
                Log.InfoRequested(logger, fileName);
                var fileInfo = await fileProvider.GetFileInfoAsync(fileName);
                if (fileInfo == null)
                {
                    Log.InfoFileNotFound(logger, fileName);
                    return Results.NotFound();
                }
                return Results.Ok(fileInfo);
            }
            catch (ArgumentException ex)
            {
                Log.InfoInvalidFileName(logger, fileName, ex);
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.InfoError(logger, fileName, ex);
                return Results.Problem(detail: ex.Message, statusCode: 500, title: "Error getting file info");
            }
        });

        return app;
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Retrieving list of available example files")]
        public static partial void ListRequested(ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error retrieving available files")]
        public static partial void ListError(ILogger logger, Exception exception);

        [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Downloading example file: {FileName}")]
        public static partial void DownloadRequested(ILogger logger, string fileName);

        [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Example file not found: {FileName}")]
        public static partial void DownloadFileNotFound(ILogger logger, string fileName, Exception exception);

        [LoggerMessage(EventId = 5, Level = LogLevel.Warning, Message = "Invalid file name: {FileName}")]
        public static partial void DownloadInvalidFileName(ILogger logger, string fileName, Exception exception);

        [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Error downloading file: {FileName}")]
        public static partial void DownloadError(ILogger logger, string fileName, Exception exception);

        [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Getting file info: {FileName}")]
        public static partial void InfoRequested(ILogger logger, string fileName);

        [LoggerMessage(EventId = 8, Level = LogLevel.Debug, Message = "File not found: {FileName}")]
        public static partial void InfoFileNotFound(ILogger logger, string fileName);

        [LoggerMessage(EventId = 9, Level = LogLevel.Warning, Message = "Invalid file name: {FileName}")]
        public static partial void InfoInvalidFileName(ILogger logger, string fileName, Exception exception);

        [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Error getting file info: {FileName}")]
        public static partial void InfoError(ILogger logger, string fileName, Exception exception);
    }
}