namespace MYInfo.Infrastructure.Persistence;

public interface IDbContext
{
#pragma warning disable CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
#pragma warning restore CA1716 // Les identificateurs ne doivent pas correspondre à des mots clés

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

