using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SpreadsheetUtility.Library.DataAccess.Repositories;

/// <summary>
/// Interface for generic repository pattern providing CRUD operations.
/// Follows the Repository pattern to abstract data access logic.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns all entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets an entity by its primary key asynchronously.
    /// </summary>
    /// <param name="id">The primary key value.</param>
    /// <returns>A task representing the asynchronous operation that returns the entity, or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// Finds entities matching a predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>A task representing the asynchronous operation that returns matching entities.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Adds multiple entities asynchronously.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes multiple entities.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Determines whether any entity matches the predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <returns>A task representing the asynchronous operation that returns true if any entity matches, otherwise false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Counts entities matching the predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The filter expression (optional).</param>
    /// <returns>A task representing the asynchronous operation that returns the count.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
}
