using MediatR;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;

namespace SpreadsheetUtility.Application.UseCases.Session;

public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, UpdateSessionResponse>
{
    private readonly IAuthService _authService;

    public UpdateSessionCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<UpdateSessionResponse> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
    {
        var updatedValue = _authService.UpdateSession(request.Email, request.SessionId, request.NewValue);
        return Task.FromResult(new UpdateSessionResponse(updatedValue));
    }
}
