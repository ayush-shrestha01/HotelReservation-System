using Hrs.Api.Repository.Data;
using Hrs.Api.Repository.RepositoryPattern;
using Microsoft.EntityFrameworkCore.Storage;

// Required for IRepository interface reference in Repository<T>() method
#pragma warning disable CS8604

namespace Hrs.Api.Repository.UnitOfWork;

/// <summary>
/// Unit of Work pattern implementation for managing repositories and database transactions.
/// </summary>
public class UnitOfWork(HrsDbContext context) : IUnitOfWork
{
    private readonly HrsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly Dictionary<string, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Gets or creates a repository instance for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A cached or new repository instance for the entity type.</returns>
    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>).MakeGenericType(typeof(T));
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(type, repositoryInstance!);
        }

        return (IRepository<T>)_repositories[type];
    }

    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously.
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Begins a database transaction asynchronously.
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    public async Task CommitAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    public async Task RollbackAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the DbContext and transaction resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}




