using Moq;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class UpdateSessionCommandHandlerTests
{
    private readonly Mock<IAuthServiceFactory> _factoryMock;
    private readonly Mock<ISessionStore> _sessionStoreMock;
    private readonly UpdateSessionCommandHandler _handler;

    public UpdateSessionCommandHandlerTests()
    {
        _factoryMock = new Mock<IAuthServiceFactory>();
        _sessionStoreMock = new Mock<ISessionStore>();
        _factoryMock.Setup(f => f.GetService(It.IsAny<CacheBackend>())).Returns(_sessionStoreMock.Object);
        _handler = new UpdateSessionCommandHandler(_factoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_UpdateSession_And_Return_UpdatedValue()
    {
        var email = "user@example.com";
        var sessionId = Guid.NewGuid();
        var newValue = "new session value";
        _sessionStoreMock.Setup(a => a.UpdateSession(email, sessionId, newValue)).Returns(newValue);

        var result = await _handler.Handle(new UpdateSessionCommand(email, sessionId, newValue), CancellationToken.None);

        Assert.Equal(newValue, result.UpdatedValue);
        _sessionStoreMock.Verify(a => a.UpdateSession(email, sessionId, newValue), Times.Once);
    }
}
