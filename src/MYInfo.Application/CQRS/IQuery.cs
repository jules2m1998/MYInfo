namespace MYInfo.Application.CQRS;

#pragma warning disable CA1040 // Éviter les interfaces vides
public interface IQuery : IRequest<ErrorOr<Unit>>;
#pragma warning restore CA1040 // Éviter les interfaces vides

#pragma warning disable CA1040 // Éviter les interfaces vides
public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>> where TResponse : notnull;
#pragma warning restore CA1040 // Éviter les interfaces vides

