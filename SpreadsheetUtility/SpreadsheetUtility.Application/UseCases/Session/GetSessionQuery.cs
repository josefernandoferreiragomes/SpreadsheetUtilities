using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record GetSessionQuery(string Email, Guid SessionId) : IRequest<GetSessionResponse>;
