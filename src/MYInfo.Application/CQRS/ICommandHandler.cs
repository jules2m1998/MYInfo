using ErrorOr;
using MediatR;

namespace MYInfo.Application.CQRS;

public interface ICommandHandler<in TCommand> :
    ICommandHandler<TCommand, Unit>
    where TCommand : notnull, ICommand<Unit>
{ }

public interface ICommandHandler<in TCommand, TResponse> :
    IRequestHandler<TCommand, ErrorOr<TResponse>>
    where TCommand : notnull, ICommand<TResponse>
    where TResponse : notnull;