namespace SpreadsheetUtility.UI.Web.Models
{
    /// <summary>
    /// Represents the complete state of a user session for the Gantt Generator.
    /// </summary>
    public class SessionState
    {
        public string Email { get; set; } = string.Empty;
        public Guid SessionId { get; set; } = Guid.Empty;

        // Data content
        public string? ProjectData { get; set; }
        public string? TaskData { get; set; }
        public string? TeamData { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public bool IsInitialized { get; set; } = false;
    }
}
