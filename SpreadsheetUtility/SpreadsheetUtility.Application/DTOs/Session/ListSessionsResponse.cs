namespace SpreadsheetUtility.Application.DTOs.Session;

public record ListSessionsResponse(IReadOnlyCollection<SessionInfoDto> Sessions);
