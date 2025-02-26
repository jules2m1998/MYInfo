using Microsoft.EntityFrameworkCore;
using MYInfo.Domain.Models;
using System.Reflection;

namespace MYInfo.Infrastructure.Persistence.Data;

public class MYInfoDbContext(DbContextOptions<MYInfoDbContext> options)
    : DbContext(options), IDbContext
{
    public DbSet<UserMetaData> UserMetaData => Set<UserMetaData>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
