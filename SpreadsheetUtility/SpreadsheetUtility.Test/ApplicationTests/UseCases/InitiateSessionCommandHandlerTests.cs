using Moq;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class InitiateSessionCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly InitiateSessionCommandHandler _handler;

    public InitiateSessionCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new InitiateSessionCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_InitiateSession_And_Return_SessionId()
    {
        var email = "user@example.com";
        var expectedSessionId = Guid.NewGuid().ToString();
        _authServiceMock.Setup(a => a.InitiateSession(email)).Returns(expectedSessionId);

        var result = await _handler.Handle(new InitiateSessionCommand(email), CancellationToken.None);

        Assert.Equal(expectedSessionId, result.SessionId);
        _authServiceMock.Verify(a => a.InitiateSession(email), Times.Once);
    }
}
