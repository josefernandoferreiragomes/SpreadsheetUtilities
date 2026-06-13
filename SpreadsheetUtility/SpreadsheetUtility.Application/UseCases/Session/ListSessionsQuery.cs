using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;

namespace SpreadsheetUtility.Application.UseCases.Session;

public record ListSessionsQuery : IRequest<ListSessionsResponse>;
