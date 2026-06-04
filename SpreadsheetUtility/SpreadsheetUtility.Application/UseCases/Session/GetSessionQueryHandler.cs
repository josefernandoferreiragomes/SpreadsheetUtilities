using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, GetSessionResponse>
{
    private readonly IAuthService _authService;

    public GetSessionQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<GetSessionResponse> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var value = _authService.GetSession(request.Email, request.SessionId);
        return Task.FromResult(new GetSessionResponse(value));
    }
}
