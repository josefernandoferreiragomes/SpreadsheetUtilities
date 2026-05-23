namespace SpreadsheetUtility.UI.Web.Models;

/// <summary>
/// Data transfer object for file download responses.
/// Contains file metadata and content.
/// </summary>
public class FileDownloadDto
{
    /// <summary>
    /// The actual file name with extension.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable display name without extension.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The file content as byte array.
    /// </summary>
    public byte[] Content { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// MIME type for the file.
    /// </summary>
    public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
