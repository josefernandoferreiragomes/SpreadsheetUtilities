using Moq;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class ListSessionsQueryHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly ListSessionsQueryHandler _handler;

    public ListSessionsQueryHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new ListSessionsQueryHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Sessions()
    {
        var sessions = new List<SessionInfoDto>
        {
            new() { Email = "user1@example.com", SessionId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, LastModifiedAt = DateTime.UtcNow },
            new() { Email = "user2@example.com", SessionId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, LastModifiedAt = DateTime.UtcNow }
        };
        _authServiceMock.Setup(a => a.GetAllSessions()).Returns(sessions);

        var result = await _handler.Handle(new ListSessionsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Sessions.Count);
        _authServiceMock.Verify(a => a.GetAllSessions(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_When_No_Sessions()
    {
        _authServiceMock.Setup(a => a.GetAllSessions()).Returns(new List<SessionInfoDto>());

        var result = await _handler.Handle(new ListSessionsQuery(), CancellationToken.None);

        Assert.Empty(result.Sessions);
    }

    [Fact]
    public async Task Handle_Should_Not_Throw_When_Service_Returns_Null()
    {
        _authServiceMock.Setup(a => a.GetAllSessions()).Returns((List<SessionInfoDto>)null!);

        var result = await _handler.Handle(new ListSessionsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result.Sessions);
    }
}