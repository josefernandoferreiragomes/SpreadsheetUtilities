using Microsoft.AspNetCore.Mvc;
using SpreadsheetUtility.Infrastructure.Abstractions;
using SpreadsheetUtility.Infrastructure.Models;

namespace SpreadsheetUtility.UI.Web.Components.Api;

/// <summary>
/// API endpoint for downloading example files.
/// Provides both listing and individual file downloads via REST API.
/// Can be used by Blazor components, external applications, or direct HTTP clients.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExampleFilesController : ControllerBase
{
    private readonly IExampleFileProvider _fileProvider;
    private readonly ILogger<ExampleFilesController> _logger;

    public ExampleFilesController(IExampleFileProvider fileProvider, ILogger<ExampleFilesController> logger)
    {
        _fileProvider = fileProvider;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of all available example files.
    /// </summary>
    /// <returns>Collection of available example files with metadata</returns>
    /// <remarks>
    /// Example response:
    /// ```json
    /// [
    ///   {
    ///     "fileName": "example1.xlsx",
    ///     "displayName": "example1",
    ///     "description": "Example spreadsheet for Gantt chart generation",
    ///     "fileSizeBytes": 15360,
    ///     "lastModified": "2024-01-15T10:30:00",
    ///     "contentType": "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ///   }
    /// ]
    /// ```
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExampleFileInfo>>> GetAvailableFiles()
    {
        try
        {
            _logger.LogInformation("Retrieving list of available example files");
            var files = await _fileProvider.GetAvailableFilesAsync();
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available files");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving files", error = ex.Message });
        }
    }

    /// <summary>
    /// Downloads a specific example file.
    /// </summary>
    /// <param name="fileName">Name of the file to download (must have .xlsx extension)</param>
    /// <returns>File download response</returns>
    /// <remarks>
    /// Example: GET /api/examplefiles/example1.xlsx
    /// Returns the file as a binary download.
    /// </remarks>
    [HttpGet("{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DownloadFile(string fileName)
    {
        try
        {
            _logger.LogInformation("Downloading example file: {FileName}", fileName);

            var fileData = await _fileProvider.GetFileAsync(fileName);

            return File(fileData.Content, fileData.ContentType, fileData.FileName);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "Example file not found: {FileName}", fileName);
            return NotFound(new { message = $"File not found: {fileName}" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid file name: {FileName}", fileName);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error downloading file", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets file information without downloading content.
    /// Useful for checking file existence or getting metadata.
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <returns>File information</returns>
    /// <remarks>
    /// Uses HTTP HEAD method which retrieves headers without downloading the entire response body.
    /// This is more efficient for checking file existence or getting metadata.
    /// </remarks>
    [HttpHead("{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetFileInfo(string fileName)
    {
        try
        {
            _logger.LogInformation("Getting file info: {FileName}", fileName);

            var fileInfo = await _fileProvider.GetFileInfoAsync(fileName);
            if (fileInfo == null)
            {
                _logger.LogDebug("File not found: {FileName}", fileName);
                return NotFound();
            }

            return Ok(fileInfo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid file name: {FileName}", fileName);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file info: {FileName}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error getting file info", error = ex.Message });
        }
    }
}
