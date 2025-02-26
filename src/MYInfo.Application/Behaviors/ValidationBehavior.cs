using ErrorOr;
using FluentValidation;
using MediatR;
using MYInfo.Application.CQRS;

namespace MYInfo.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, ErrorOr<TResponse>>
    where TRequest : notnull, ICommand<TResponse>
    where TResponse : notnull
{
    public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validaionResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );
        var failures = validaionResults
            .Where(v => v.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            var errors = failures.Select(error => Error.Validation(error.PropertyName, error.ErrorMessage)).ToList();

            return errors;
        }

        return await next();
    }
}
