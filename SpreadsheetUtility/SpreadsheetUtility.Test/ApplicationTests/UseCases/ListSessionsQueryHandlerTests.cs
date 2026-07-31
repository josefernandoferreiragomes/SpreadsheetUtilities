using Moq;
using SpreadsheetUtility.Application.DTOs.Session;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Application.UseCases.Session;

namespace SpreadsheetUtility.Test.ApplicationTests.UseCases;

public class ListSessionsQueryHandlerTests
{
    private readonly Mock<IAuthServiceFactory> _factoryMock;
    private readonly Mock<ISessionStore> _sessionStoreMock;
    private readonly ListSessionsQueryHandler _handler;

    public ListSessionsQueryHandlerTests()
    {
        _factoryMock = new Mock<IAuthServiceFactory>();
        _sessionStoreMock = new Mock<ISessionStore>();
        _factoryMock.Setup(f => f.GetService(It.IsAny<CacheBackend>())).Returns(_sessionStoreMock.Object);
        _handler = new ListSessionsQueryHandler(_factoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Sessions()
    {
        var sessions = new List<SessionInfoDto>
        {
            new() { Email = "user1@example.com", SessionId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, LastModifiedAt = DateTime.UtcNow },
            new() { Email = "user2@example.com", SessionId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, LastModifiedAt = DateTime.UtcNow }
        };
        _sessionStoreMock.Setup(a => a.GetAllSessions()).Returns(sessions);

        var result = await _handler.Handle(new ListSessionsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Sessions.Count);
        _sessionStoreMock.Verify(a => a.GetAllSessions(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_When_No_Sessions()
    {
        _sessionStoreMock.Setup(a => a.GetAllSessions()).Returns(new List<SessionInfoDto>());

        var result = await _handler.Handle(new ListSessionsQuery(), CancellationToken.None);

        Assert.Empty(result.Sessions);
    }

    [Fact]
    public async Task Handle_Should_Not_Throw_When_Service_Returns_Null()
    {
        _sessionStoreMock.Setup(a => a.GetAllSessions()).Returns((List<SessionInfoDto>)null!);

        var result = await _handler.Handle(new ListSessionsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result.Sessions);
    }
}
