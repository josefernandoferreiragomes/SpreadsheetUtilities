using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SpreadsheetUtility.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogDebug("Handling {RequestName}: {@Request}", requestName, request);

        var response = await next();

        stopwatch.Stop();
        logger.LogDebug("Handled {RequestName} in {ElapsedMs}ms: {@Response}", requestName, stopwatch.ElapsedMilliseconds, response);

        return response;
    }
}
