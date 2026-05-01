using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpreadsheetUtility.Library.DataAccess.DbContexts;
using SpreadsheetUtility.Library.DataAccess.Repositories;

namespace SpreadsheetUtility.Library.DataAccess.UnitOfWork;

/// <summary>
/// Unit of Work implementation that coordinates multiple repositories and manages transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = [];
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets or creates a repository for the specified entity type.
    /// </summary>
    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(type);
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context)
                ?? throw new InvalidOperationException($"Failed to create repository for {type.Name}");
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<TEntity>)_repositories[type];
    }

    /// <summary>
    /// Saves all changes to the database asynchronously.
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    public async Task<IAsyncDisposable> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            await _transaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _transaction?.RollbackAsync()!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the context and clears repositories.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _repositories.Clear();
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
