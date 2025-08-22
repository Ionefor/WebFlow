using System.Threading;
using System.Threading.Tasks;

namespace WebFlow.Abstractions.Interfaces;

public interface IQueryHandler<TResponse, in TQuery> where TQuery : IQuery
{
    public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default);
}