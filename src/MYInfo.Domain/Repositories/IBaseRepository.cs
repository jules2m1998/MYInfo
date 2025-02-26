namespace MYInfo.Domain.Repositories;

public interface IBaseRepository<TEntity, TId>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
    Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);
    IQueryable<TEntity> Queryable();
}