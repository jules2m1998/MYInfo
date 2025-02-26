using ErrorOr;
using MediatR;

namespace MYInfo.Application.CQRS;

public interface IQueryHandler<in TQuery> :
    IQueryHandler<TQuery, Unit>
    where TQuery : notnull, IQuery<Unit>;

public interface IQueryHandler<in TQuery, TResponse> :
    IRequestHandler<TQuery, ErrorOr<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull;
