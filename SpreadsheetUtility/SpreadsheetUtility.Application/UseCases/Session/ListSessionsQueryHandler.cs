using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class ListSessionsQueryHandler : IRequestHandler<ListSessionsQuery, ListSessionsResponse>
{
    private readonly IAuthService _authService;

    public ListSessionsQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<ListSessionsResponse> Handle(ListSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _authService.GetAllSessions();
        return Task.FromResult(new ListSessionsResponse(sessions));
    }
}
