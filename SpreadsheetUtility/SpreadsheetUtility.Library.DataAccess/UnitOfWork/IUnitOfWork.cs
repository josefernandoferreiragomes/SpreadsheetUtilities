using SpreadsheetUtility.Library.DataAccess.Repositories;

namespace SpreadsheetUtility.Library.DataAccess.UnitOfWork;

/// <summary>
/// Interface for Unit of Work pattern that coordinates multiple repositories.
/// Implements the Unit of Work pattern to manage transactions and ensure data consistency.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Gets the generic repository for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>A repository for the entity type.</returns>
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

    /// <summary>
    /// Saves all changes to the database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns the number of changes saved.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns a transaction.</returns>
    Task<IAsyncDisposable> BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync();
}
