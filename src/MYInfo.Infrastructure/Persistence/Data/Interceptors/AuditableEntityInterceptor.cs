using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MYInfo.Domain.Abstractions;
using MYInfo.Domain.Services;

namespace MYInfo.Infrastructure.Persistence.Data.Interceptors;


public class AuditableEntityInterceptor(IUserContextService userContextService) : SaveChangesInterceptor
{
    private string _userIdentifier => userContextService.GetUserIdentifier();

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
        )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
        )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(
        DbContext? context
        )
    {
        if (context == null) return;

        foreach (var item in context.ChangeTracker.Entries<IEntity>())
        {
            if (item.State == EntityState.Added)
            {
                item.Entity.CreatedAt = DateTime.UtcNow;
                item.Entity.CreatedBy = _userIdentifier;
            }

            if (item.State == EntityState.Added || item.State == EntityState.Modified || item.HasChangedOwnedEntities())
            {
                item.Entity.LastModified = DateTime.UtcNow;
                item.Entity.LastModifiedBy = _userIdentifier;
            }

        }

        foreach (var item in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (item.State == EntityState.Deleted)
            {
                item.Entity.DeletedAt = DateTime.UtcNow;
                item.Entity.DeletedBy = _userIdentifier;
                item.State = EntityState.Modified;
            }
            else if (item.State == EntityState.Modified && item.Entity.DeletedAt is not null)
            {
                item.State = EntityState.Unchanged;
            }
        }
    }
}


public static class InterceptorExtensions
{
    public static bool HasChangedOwnedEntities(
        this EntityEntry entry
        ) =>
        entry.References.Any(r =>
            r.TargetEntry != null
            && r.TargetEntry.Metadata.IsOwned()
            && (
                r.TargetEntry.State == EntityState.Added
                || r.TargetEntry.State == EntityState.Modified
            )
        );
}
