using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace WebFlow.Abstractions.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}