using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, GetSessionResponse>
{
    private readonly IAuthServiceFactory _factory;

    public GetSessionQueryHandler(IAuthServiceFactory factory)
    {
        _factory = factory;
    }

    public Task<GetSessionResponse> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var sessionStore = _factory.GetService(request.cache);
        var value = sessionStore.GetSession(request.Email, request.SessionId);
        return Task.FromResult(new GetSessionResponse(value));
    }
}
