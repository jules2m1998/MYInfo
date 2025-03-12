using Microsoft.EntityFrameworkCore;
using Moq;
using MYInfo.Domain.Services;
using MYInfo.Infrastructure.Persistence.Data.Interceptors;

namespace MYInfo.InfrastructureUnitTests.Data.Interceptors;

public class AuditableEntityInterceptorTests
{
    private readonly DbContextOptions<TestDbContext> options;
    private const string USER_IDENTIFIER = "TESTER";

    public AuditableEntityInterceptorTests()
    {
        var userContext = new Mock<IUserContextService>();
        _ = userContext.Setup(x => x.GetUserIdentifier()).Returns(USER_IDENTIFIER);
        options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditableEntityInterceptor(userContext.Object))
            .Options;
    }

    [Fact]
    public async Task SaveChangesAsync_NewItem_UpdatesCreatedAtToNow()
    {
        // Arrange
        using var context = new TestDbContext(options);
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        var testEntity = new EntityImplementation { Id = 1 };
        _ = context.EntityImpls.Add(testEntity);

        // Act
        _ = await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(testEntity.CreatedAt);
        Assert.True((DateTime.UtcNow - testEntity.CreatedAt)?.TotalMinutes < 1, "CreatedAt should be within the last minute");
        Assert.NotNull(testEntity.LastModified);
        Assert.True((DateTime.UtcNow - testEntity.LastModified)?.TotalMinutes < 1, "LastModified should be within the last minute");

        Assert.Equal(USER_IDENTIFIER, testEntity.CreatedBy);
        Assert.Equal(USER_IDENTIFIER, testEntity.LastModifiedBy);
    }

    [Fact]
    public async Task SaveChangesAsync_EditItem_UpdatesLastModifiedToNow()
    {
        // Arrange
        using var context = new TestDbContext(options);
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        var testEntity = new EntityImplementation { Id = 1 };
        _ = context.EntityImpls.Add(testEntity);
        _ = await context.SaveChangesAsync();

        // Act
        testEntity.Name = "Test";
        _ = await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(testEntity.LastModified);
        Assert.True((DateTime.UtcNow - testEntity.LastModified)?.TotalMinutes < 1, "LastModified should be within the last minute");

        Assert.Equal(USER_IDENTIFIER, testEntity.CreatedBy);
        Assert.Equal(USER_IDENTIFIER, testEntity.LastModifiedBy);
    }

    [Fact]
    public async Task SaveChangesAsync_DeleteItem_SetDeletedAtToNow()
    {
        // Arrange
        using var context = new TestDbContext(options);
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        var testEntity = new EntityDeletableImpl { Id = 1 };
        _ = context.Add(testEntity);
        _ = await context.SaveChangesAsync();

        // Act
        _ = context.EntityDeletableImpls.Remove(testEntity);
        _ = await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(testEntity.DeletedAt);
        Assert.True((DateTime.UtcNow - testEntity.DeletedAt)?.TotalMinutes < 1, "LastModified should be within the last minute");

        Assert.Equal(USER_IDENTIFIER, testEntity.CreatedBy);
        Assert.Equal(USER_IDENTIFIER, testEntity.LastModifiedBy);
        Assert.Equal(USER_IDENTIFIER, testEntity.DeletedBy);
    }

    [Fact]
    public async Task SaveChangesAsync_UpdateItem_NotApplyChanges()
    {
        // Arrange
        using var context = new TestDbContext(options);
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        var testEntity = new EntityDeletableImpl { Id = 1, Name = "Before", DeletedAt = DateTime.UtcNow };
        _ = context.Add(testEntity);
        _ = await context.SaveChangesAsync();
        _ = context.EntityDeletableImpls.Remove(testEntity);

        // Act
        testEntity.Name = "After";
        _ = context.EntityDeletableImpls.Update(testEntity);
        _ = await context.SaveChangesAsync();

        // Assert
        var entity = await context.EntityDeletableImpls.FindAsync(testEntity.Id);
        Assert.NotNull(entity);
        Assert.Equal("Before", entity!.Name);
    }
}
