namespace MYInfo.Application.CQRS;

#pragma warning disable CA1040 // Éviter les interfaces vides
public interface ICommand : IRequest<ErrorOr<Unit>>;
#pragma warning restore CA1040 // Éviter les interfaces vides

#pragma warning disable CA1040 // Éviter les interfaces vides
public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>
#pragma warning restore CA1040 // Éviter les interfaces vides
    where TResponse : notnull
{ }
