using Hrs.Api.Repository.RepositoryPattern;

namespace Hrs.Api.Repository.UnitOfWork;

/// <summary>
/// Unit of Work pattern interface for managing multiple repositories and database transactions.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Gets a repository instance for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A repository instance for the entity type.</returns>
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a database transaction asynchronously.
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    Task RollbackAsync();
}

