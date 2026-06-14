using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record GetSessionQuery(string Email, Guid SessionId, CacheBackend cache = CacheBackend.Memory) : IRequest<GetSessionResponse>;
