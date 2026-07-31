using Moq;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class InitiateSessionCommandHandlerTests
{
    private readonly Mock<IAuthServiceFactory> _factoryMock;
    private readonly Mock<ISessionStore> _sessionStoreMock;
    private readonly InitiateSessionCommandHandler _handler;

    public InitiateSessionCommandHandlerTests()
    {
        _sessionStoreMock = new Mock<ISessionStore>();
        _factoryMock = new Mock<IAuthServiceFactory>();
        _factoryMock.Setup(f => f.GetService(It.IsAny<CacheBackend>())).Returns(_sessionStoreMock.Object);
        _handler = new InitiateSessionCommandHandler(_factoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_InitiateSession_And_Return_SessionId()
    {
        var email = "user@example.com";
        var expectedSessionId = Guid.NewGuid().ToString();
        _sessionStoreMock.Setup(s => s.InitiateSession(email)).Returns(expectedSessionId);

        var result = await _handler.Handle(new InitiateSessionCommand(email, null, CacheBackend.Memory), CancellationToken.None);

        Assert.Equal(expectedSessionId, result.SessionId);
        _sessionStoreMock.Verify(s => s.InitiateSession(email), Times.Once);
        _factoryMock.Verify(f => f.GetService(CacheBackend.Memory), Times.Once);
    }
}
