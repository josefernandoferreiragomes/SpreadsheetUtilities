namespace SpreadsheetUtility.Application.DTOs.Session;

public record SessionInfoDto
{
    public string Email { get; init; } = string.Empty;
    public Guid SessionId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastModifiedAt { get; init; }
    public string? SessionData { get; init; }
}
