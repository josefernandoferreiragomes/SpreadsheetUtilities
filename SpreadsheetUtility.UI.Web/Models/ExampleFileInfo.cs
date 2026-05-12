namespace SpreadsheetUtility.UI.Web.Models;

/// <summary>
/// Information about an example file.
/// Contains metadata for browsing and displaying available files.
/// </summary>
public class ExampleFileInfo
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
    /// Description of what this example demonstrates.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Last time the file was modified.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// MIME type for the file.
    /// </summary>
    public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
