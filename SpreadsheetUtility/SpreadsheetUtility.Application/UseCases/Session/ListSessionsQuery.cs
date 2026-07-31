using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record ListSessionsQuery(CacheBackend cache = CacheBackend.Memory) : IRequest<ListSessionsResponse>;
