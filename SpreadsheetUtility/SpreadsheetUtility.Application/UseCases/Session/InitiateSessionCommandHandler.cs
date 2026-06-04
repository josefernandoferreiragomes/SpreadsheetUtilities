using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class InitiateSessionCommandHandler : IRequestHandler<InitiateSessionCommand, InitiateSessionResponse>
{
    private readonly IAuthService _authService;

    public InitiateSessionCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<InitiateSessionResponse> Handle(InitiateSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionId = _authService.InitiateSession(request.Email);
        return Task.FromResult(new InitiateSessionResponse(sessionId));
    }
}
