using ErrorOr;

namespace MYInfo.API.Extensions;

public static class ErrorOrExtensions
{
    public static ProblemDetails? ToProblemDetails<T>(this ErrorOr<T> errorOr)
    {
        if (!errorOr.IsError)
        {
            return null;
        }

        var firstError = errorOr.FirstError;
        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return new()
        {
            Status = statusCode,
            Title = firstError.Code,
            Detail = firstError.Description
        };
    }
}
