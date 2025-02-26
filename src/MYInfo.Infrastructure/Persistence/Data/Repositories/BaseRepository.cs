using Microsoft.EntityFrameworkCore;
using MYInfo.Domain.Abstractions;
using MYInfo.Domain.Repositories;

namespace MYInfo.Infrastructure.Persistence.Data.Repositories;


public class BaseRepository<TEntity, TId>(IDbContext context) : IBaseRepository<TEntity, TId>
    where TEntity : class
{
    protected DbSet<TEntity> Entity => context.Set<TEntity>();


    public async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _ = Entity.Add(entity);
        return await SaveAsync(1, cancellationToken);
    }

    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var exists = await Entity.AnyAsync(e => e == entity, cancellationToken);
        if (!exists)
        {
            return false;
        }

        _ = Entity.Remove(entity);
        return await SaveAsync(1, cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            return await DeleteAsync(entity, cancellationToken);
        }

        return false;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
    {
        var query = Entity.AsQueryable();
        if (excludeDeleted && typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
        {
            query = Entity.Where(entity => EF.Property<DateTime?>(entity, nameof(ISoftDeletable.DeletedAt)) == null);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        var entity = await Entity.FindAsync(new object[] { id }, cancellationToken);
        if (entity is ISoftDeletable deletable && deletable.DeletedAt is not null)
        {
            return null;
        }
        return entity;
    }

    public IQueryable<TEntity> Queryable() => Entity;

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var exists = await Entity.AnyAsync(e => e == entity, cancellationToken);
        if (!exists)
        {
            return false;
        }
        _ = Entity.Update(entity);
        return await SaveAsync(1, cancellationToken);
    }


    #region protected
    protected async Task<bool> SaveAsync(int? equalTo = null, CancellationToken cancellationToken = default)
    {
        var result = await context.SaveChangesAsync(cancellationToken);
        if (equalTo is not null) return equalTo == result;
        return result > 0;
    }
    #endregion
}