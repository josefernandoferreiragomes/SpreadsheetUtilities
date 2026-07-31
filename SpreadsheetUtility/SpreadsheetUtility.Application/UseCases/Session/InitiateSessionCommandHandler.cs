using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class InitiateSessionCommandHandler : IRequestHandler<InitiateSessionCommand, InitiateSessionResponse>
{   
    private readonly IAuthServiceFactory _factory;

    public InitiateSessionCommandHandler(IAuthServiceFactory factory)
    {        
        _factory = factory;        
    }

    public Task<InitiateSessionResponse> Handle(InitiateSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionStore = _factory.GetService(request.cache);
        var sessionId = sessionStore.InitiateSession(request.Email);
        return Task.FromResult(new InitiateSessionResponse(sessionId));
    }
}
