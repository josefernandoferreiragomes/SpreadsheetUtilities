using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, UpdateSessionResponse>
{
    private readonly IAuthServiceFactory _factory;

    public UpdateSessionCommandHandler(IAuthServiceFactory factory)
    {
        _factory = factory;
    }

    public Task<UpdateSessionResponse> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionStore = _factory.GetService(request.cache);
        var updatedValue = sessionStore.UpdateSession(request.Email, request.SessionId, request.NewValue);
        return Task.FromResult(new UpdateSessionResponse(updatedValue));
    }
}
