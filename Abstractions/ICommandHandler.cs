using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ErrorsFlow.Models;

namespace WebFlow.Abstractions;

public interface ICommandHandler<TResponse, in TCommand> where TCommand : ICommand
{
    public Task<Result<TResponse, ErrorList>> Handle
        (TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public Task<UnitResult<ErrorList>> Handle
        (TCommand command, CancellationToken cancellationToken = default);
}