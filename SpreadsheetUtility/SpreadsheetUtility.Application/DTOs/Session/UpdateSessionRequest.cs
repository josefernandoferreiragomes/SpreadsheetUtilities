namespace SpreadsheetUtility.Application.DTOs.Session;

public record UpdateSessionRequest(string Email, Guid SessionId, string NewValue);
