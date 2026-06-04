using Moq;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class UpdateSessionCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly UpdateSessionCommandHandler _handler;

    public UpdateSessionCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new UpdateSessionCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_UpdateSession_And_Return_UpdatedValue()
    {
        var email = "user@example.com";
        var sessionId = Guid.NewGuid();
        var newValue = "new session value";
        _authServiceMock.Setup(a => a.UpdateSession(email, sessionId, newValue)).Returns(newValue);

        var result = await _handler.Handle(new UpdateSessionCommand(email, sessionId, newValue), CancellationToken.None);

        Assert.Equal(newValue, result.UpdatedValue);
        _authServiceMock.Verify(a => a.UpdateSession(email, sessionId, newValue), Times.Once);
    }
}
