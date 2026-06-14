using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class ListSessionsQueryHandler : IRequestHandler<ListSessionsQuery, ListSessionsResponse>
{
    private readonly IAuthServiceFactory _factory;

    public ListSessionsQueryHandler(IAuthServiceFactory factory)
    {
        _factory = factory;
    }

    public Task<ListSessionsResponse> Handle(ListSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessionStore = _factory.GetService(request.cache);
        var sessions = sessionStore.GetAllSessions();
        return Task.FromResult(new ListSessionsResponse(sessions));
    }
}
