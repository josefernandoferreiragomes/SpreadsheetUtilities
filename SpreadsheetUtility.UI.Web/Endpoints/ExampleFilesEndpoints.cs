using SpreadsheetUtility.Infrastructure.Abstractions;
using SpreadsheetUtility.Infrastructure.Models;

namespace SpreadsheetUtility.UI.Web.Endpoints;

public static class ExampleFilesEndpoints
{
    public static IEndpointRouteBuilder MapExampleFilesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/examplefiles")
            .WithTags("ExampleFiles");

        // GET /api/examplefiles — list all available files
        group.MapGet("/", async (IExampleFileProvider fileProvider, ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("ExampleFiles");
            try
            {
                logger.LogInformation("Retrieving list of available example files");
                var files = await fileProvider.GetAvailableFilesAsync();
                return Results.Ok(files);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving available files");
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
                logger.LogInformation("Downloading example file: {FileName}", fileName);
                var fileData = await fileProvider.GetFileAsync(fileName);
                return Results.File(fileData.Content, fileData.ContentType, fileData.FileName);
            }
            catch (FileNotFoundException ex)
            {
                logger.LogWarning(ex, "Example file not found: {FileName}", fileName);
                return Results.NotFound(new { message = $"File not found: {fileName}" });
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid file name: {FileName}", fileName);
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error downloading file: {FileName}", fileName);
                return Results.Problem(detail: ex.Message, statusCode: 500, title: "Error downloading file");
            }
        });

        // HEAD /api/examplefiles/{fileName} — get file info
        group.MapMethods("/{fileName}", new[] { "HEAD" }, async (string fileName, IExampleFileProvider fileProvider, ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("ExampleFiles");
            try
            {
                logger.LogInformation("Getting file info: {FileName}", fileName);
                var fileInfo = await fileProvider.GetFileInfoAsync(fileName);
                if (fileInfo == null)
                {
                    logger.LogDebug("File not found: {FileName}", fileName);
                    return Results.NotFound();
                }
                return Results.Ok(fileInfo);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid file name: {FileName}", fileName);
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting file info: {FileName}", fileName);
                return Results.Problem(detail: ex.Message, statusCode: 500, title: "Error getting file info");
            }
        });

        return app;
    }
}
