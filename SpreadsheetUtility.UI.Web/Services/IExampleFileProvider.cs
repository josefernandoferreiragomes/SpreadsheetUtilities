namespace SpreadsheetUtility.UI.Web.Services;

using SpreadsheetUtility.UI.Web.Models;

/// <summary>
/// Abstraction layer for providing example files.
/// This design allows easy switching between implementations:
/// - FolderExampleFileProvider (current): Serves from local wwwroot folder
/// - FileServerExampleFileProvider (future): Serves from dedicated file server
/// </summary>
public interface IExampleFileProvider
{
    /// <summary>
    /// Gets all available example files.
    /// </summary>
    /// <returns>Collection of available example files with metadata.</returns>
    Task<IEnumerable<ExampleFileInfo>> GetAvailableFilesAsync();

    /// <summary>
    /// Gets a specific example file by name, including its content.
    /// </summary>
    /// <param name="fileName">Name of the file to retrieve.</param>
    /// <returns>File with content ready for download.</returns>
    /// <exception cref="FileNotFoundException">Thrown if file does not exist.</exception>
    /// <exception cref="ArgumentException">Thrown if fileName is invalid.</exception>
    Task<FileDownloadDto> GetFileAsync(string fileName);

    /// <summary>
    /// Gets file details without downloading the content.
    /// Useful for checking file existence or getting metadata.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>File information, or null if not found.</returns>
    /// <exception cref="ArgumentException">Thrown if fileName is invalid.</exception>
    Task<ExampleFileInfo?> GetFileInfoAsync(string fileName);
}
