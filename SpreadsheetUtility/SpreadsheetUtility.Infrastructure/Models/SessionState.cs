namespace SpreadsheetUtility.Infrastructure.Models;

public class SessionState
{
    public string Email { get; set; } = string.Empty;
    public Guid SessionId { get; set; } = Guid.Empty;
    public string? ProjectData { get; set; }
    public string? TaskData { get; set; }
    public string? TeamData { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public bool IsInitialized { get; set; } = false;
}
