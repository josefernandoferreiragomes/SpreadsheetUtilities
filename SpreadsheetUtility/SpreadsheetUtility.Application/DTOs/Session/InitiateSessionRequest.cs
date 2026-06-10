namespace SpreadsheetUtility.Application.DTOs.Session;

public record InitiateSessionRequest(string Email, Guid? Guid = null);
