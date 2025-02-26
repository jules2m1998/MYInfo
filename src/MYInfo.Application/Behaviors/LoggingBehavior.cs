using MediatR;
using Microsoft.Extensions.Logging;
using MYInfo.Application.CQRS;
using System.Diagnostics;

namespace MYInfo.Application.Behaviors;


public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, ICommand<TResponse>
    where TResponse : notnull
{
    private static readonly Action<ILogger, string, string, object, Exception?> _logHandlerStart =
        LoggerMessage.Define<string, string, object>(
            LogLevel.Information,
            new EventId(1, "HandlerStart"),
            "[START] Handler request={Request} - Response={Response} - RequestData={RequestData}");

    // Define the log message for the performance warning
    private static readonly Action<ILogger, string, int, Exception?> _logPerformanceWarning =
        LoggerMessage.Define<string, int>(
            LogLevel.Warning,
            new EventId(1001, "PerformanceWarning"),
            "[PERFORMANCE] The request {Request} took {TimeTaken} seconds.");

    // Define the log message for the request end
    private static readonly Action<ILogger, string, object, Exception?> _logRequestEnd =
        LoggerMessage.Define<string, object>(
            LogLevel.Information,
            new EventId(1002, "RequestEnd"),
            "[END] Handler request={Request} with {Response}");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _logHandlerStart(logger, typeof(TRequest).Name, typeof(TResponse).Name, request, null);
        var timer = new Stopwatch();
        timer.Start();

        ArgumentNullException.ThrowIfNull(next);

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3)
        {
            _logPerformanceWarning(logger, typeof(TRequest).Name, timeTaken.Seconds, null);
        }
        _logRequestEnd(logger, typeof(TRequest).Name, response, null);

        return response;
    }
}

