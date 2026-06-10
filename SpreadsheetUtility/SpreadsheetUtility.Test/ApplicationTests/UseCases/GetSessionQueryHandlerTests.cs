using Moq;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class GetSessionQueryHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly GetSessionQueryHandler _handler;

    public GetSessionQueryHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new GetSessionQueryHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_GetSession_And_Return_Value()
    {
        var email = "user@example.com";
        var sessionId = Guid.NewGuid();
        var expectedValue = "session value";
        _authServiceMock.Setup(a => a.GetSession(email, sessionId)).Returns(expectedValue);

        var result = await _handler.Handle(new GetSessionQuery(email, sessionId), CancellationToken.None);

        Assert.Equal(expectedValue, result.SessionValue);
        _authServiceMock.Verify(a => a.GetSession(email, sessionId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Session_Not_Found()
    {
        var email = "unknown@example.com";
        var sessionId = Guid.NewGuid();
        _authServiceMock.Setup(a => a.GetSession(email, sessionId)).Returns((string?)null);

        var result = await _handler.Handle(new GetSessionQuery(email, sessionId), CancellationToken.None);

        Assert.Null(result.SessionValue);
    }
}
