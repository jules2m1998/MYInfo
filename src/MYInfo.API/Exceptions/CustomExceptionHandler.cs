namespace MYInfo.API.Exceptions;


public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    private static readonly Action<ILogger, string, DateTime, Exception?> LogError =
        LoggerMessage.Define<string, DateTime>(
            LogLevel.Error,
            new EventId(id: 0, name: "ErrorOccurred"),
            "Error Message: {ExceptionMessage}, time of occurrence {Time}"
        );
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LogError(logger, exception.Message, DateTime.UtcNow, exception);
        int statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
        httpContext.Response.StatusCode = statusCode;

        var pbDetails = new ProblemDetails
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Status = statusCode,
            Instance = httpContext.Request.Path,
        };

        pbDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);

        if (exception is ValidationException validationException)
        {
            pbDetails.Extensions.Add("ValidationErrors", validationException.Errors);
        }

        await httpContext.Response.WriteAsJsonAsync(pbDetails, cancellationToken);

        return true;
    }
}
