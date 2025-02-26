using Microsoft.EntityFrameworkCore;
using MYInfo.Domain.Abstractions;
using MYInfo.Infrastructure.Persistence;

namespace MYInfo.InfrastructureUnitTests.Data;

internal class EntityImplementation : Entity<int>
{
    public string Name { get; set; } = string.Empty;
}

internal sealed class EntityDeletableImpl : EntityImplementation, ISoftDeletable
{
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

internal sealed class TestDbContext(DbContextOptions options) : DbContext(options), IDbContext
{
    public DbSet<EntityImplementation> EntityImpls { get; set; }
    public DbSet<EntityDeletableImpl> EntityDeletableImpls { get; set; }

    public static TestDbContext GetInMemoryTestDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.CreateVersion7().ToString())
            .Options;
        return new TestDbContext(options);
    }

}