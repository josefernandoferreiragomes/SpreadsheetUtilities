using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SpreadsheetUtility.Library.DataAccess.Repositories;

/// <summary>
/// Generic repository implementation providing CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public GenericRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Gets an entity by its primary key asynchronously.
    /// </summary>
    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Finds entities matching a predicate asynchronously.
    /// </summary>
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    public async Task AddAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// Adds multiple entities asynchronously.
    /// </summary>
    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _dbSet.AddRangeAsync(entities);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Removes an entity.
    /// </summary>
    public void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Removes multiple entities.
    /// </summary>
    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Determines whether any entity matches the predicate asynchronously.
    /// </summary>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    /// <summary>
    /// Counts entities matching the predicate asynchronously.
    /// </summary>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        if (predicate is null)
        {
            return await _dbSet.CountAsync();
        }

        return await _dbSet.CountAsync(predicate);
    }
}
