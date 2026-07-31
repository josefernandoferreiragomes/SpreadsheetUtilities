using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SpreadsheetUtility.Application.Behaviors;

public partial class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        Log.HandlingRequest(logger, requestName, request);

        var response = await next();

        stopwatch.Stop();

        if (response is null)
        {
            Log.HandlingRequest(logger, requestName, "Response is null");
            throw new InvalidOperationException($"Response for {requestName} is null.");
        }
        Log.HandledRequest(logger, requestName, stopwatch.ElapsedMilliseconds, response);

        return response;
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Handling {RequestName}: {@Request}")]
        public static partial void HandlingRequest(ILogger logger, string requestName, object request);

        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Handled {RequestName} in {ElapsedMs}ms: {@Response}")]
        public static partial void HandledRequest(ILogger logger, string requestName, long elapsedMs, object response);
    }
}