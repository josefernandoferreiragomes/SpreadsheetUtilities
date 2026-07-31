using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using SpreadsheetUtility.Application.Ports;
using SpreadsheetUtility.Infrastructure.Options;
using SpreadsheetUtility.Infrastructure.Services;

namespace SpreadsheetUtility.Test.InfrastructureTests;

public class LlmTransformerServiceTests
{
    private readonly Mock<IOptions<LlmOptions>> _optionsMock;
    private readonly LlmOptions _options;

    public LlmTransformerServiceTests()
    {
        _options = new LlmOptions
        {
            BaseUrl = "http://localhost:1234",
            Model = "qwen2.5-3b-instruct",
            TimeoutSeconds = 60
        };
        _optionsMock = new Mock<IOptions<LlmOptions>>();
        _optionsMock.Setup(o => o.Value).Returns(_options);
    }

    [Fact]
    public async Task TransformAsync_Projects_ReturnsTransformedData()
    {
        // Arrange — LM Studio returns output as an array of { type, content } objects
        var llmResponse = new
        {
            output = new[]
            {
                new { type = "message", content = "ProjectID\tProject Name\tProject Group Id\tTeam Id\n1\tProject A\t1\t1\n2\tProject B\t1\t2" }
            }
        };
        var httpClient = CreateMockHttpClient(llmResponse);

        var service = new LlmTransformerService(httpClient, _optionsMock.Object);

        // Act
        var result = await service.TransformAsync("some input data", TargetFormat.Projects);

        // Assert
        Assert.True(result.IsPossible);
        Assert.Contains("ProjectID", result.Output);
        Assert.Contains("Project A", result.Output);
        Assert.Null(result.ErrorReason);
    }

    [Fact]
    public async Task TransformAsync_Tasks_ReturnsTransformedData()
    {
        // Arrange
        var llmResponse = new
        {
            output = new[]
            {
                new { type = "message", content = "ID\tProject Id\tProjectName\tTaskName\tEstimatedEffortHours\tDependencies\tProgress\tInternalID\n1\t1\tProject A\tDesign UI\t100\t\t50\t1234" }
            }
        };
        var httpClient = CreateMockHttpClient(llmResponse);

        var service = new LlmTransformerService(httpClient, _optionsMock.Object);

        // Act
        var result = await service.TransformAsync("some input data", TargetFormat.Tasks);

        // Assert
        Assert.True(result.IsPossible);
        Assert.Contains("TaskName", result.Output);
        Assert.Contains("Design UI", result.Output);
        Assert.Null(result.ErrorReason);
    }

    [Fact]
    public async Task TransformAsync_Team_ReturnsTransformedData()
    {
        // Arrange
        var llmResponse = new
        {
            output = new[]
            {
                new { type = "message", content = "Team ID\tTeam Name\tDeveloper Id\tDeveloper Name\tDeveloper Vacation Date Intervals\tDaily Work Hours\n1\tTeam Alpha\t1\tAlice\t2026-08-10;2026-08-15|\t6" }
            }
        };
        var httpClient = CreateMockHttpClient(llmResponse);

        var service = new LlmTransformerService(httpClient, _optionsMock.Object);

        // Act
        var result = await service.TransformAsync("some input data", TargetFormat.Team);

        // Assert
        Assert.True(result.IsPossible);
        Assert.Contains("Team ID", result.Output);
        Assert.Contains("Team Alpha", result.Output);
        Assert.Null(result.ErrorReason);
    }

    [Fact]
    public async Task TransformAsync_ImpossibleEtl_ReturnsErrorReason()
    {
        // Arrange
        var llmResponse = new
        {
            output = new[]
            {
                new { type = "message", content = "IMPOSSIBLE:|The input data does not contain project-related columns. Expected columns like project ID, project name, etc." }
            }
        };
        var httpClient = CreateMockHttpClient(llmResponse);

        var service = new LlmTransformerService(httpClient, _optionsMock.Object);

        // Act
        var result = await service.TransformAsync("some unrelated data", TargetFormat.Projects);

        // Assert
        Assert.False(result.IsPossible);
        Assert.Empty(result.Output);
        Assert.Contains("does not contain project-related columns", result.ErrorReason);
    }

    [Fact]
    public async Task TransformAsync_EmptyInput_ReturnsError()
    {
        // Arrange
        var httpClient = new HttpClient(new Mock<HttpMessageHandler>().Object);
        var service = new LlmTransformerService(httpClient, _optionsMock.Object);

        // Act
        var result = await service.TransformAsync("   ", TargetFormat.Projects);

        // Assert
        Assert.False(result.IsPossible);
        Assert.Contains("empty", result.ErrorReason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TransformAsync_EmptyLlmResponse_ReturnsError()
    {
        // Arrange — empty output array ? no message content
        var llmResponse = new
        {
            output = Array.Empty<object>()
        };
        var httpClient = CreateMockHttpClient(llmResponse);

        var service = new LlmTransformerService(httpClient, _optionsMock.Object);

        // Act
        var result = await service.TransformAsync("some data", TargetFormat.Projects);

        // Assert
        Assert.False(result.IsPossible);
        Assert.Contains("empty", result.ErrorReason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TransformAsync_HttpError_ReturnsErrorMessage()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));
        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri(_options.BaseUrl)
        };

        var service = new LlmTransformerService(httpClient, _optionsMock.Object);

        // Act
        var result = await service.TransformAsync("some data", TargetFormat.Projects);

        // Assert
        Assert.False(result.IsPossible);
        Assert.Contains("Connection refused", result.ErrorReason);
    }

    private HttpClient CreateMockHttpClient(object llmResponse)
    {
        var json = JsonSerializer.Serialize(llmResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri(_options.BaseUrl)
        };
    }
}