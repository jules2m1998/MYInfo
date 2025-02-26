using AutoFixture;
using Microsoft.EntityFrameworkCore;
using MYInfo.Infrastructure.Persistence.Data.Repositories;

namespace MYInfo.InfrastructureUnitTests.Data.Repositories;

public sealed class BaseRepositoryTests
{
    private readonly TestDbContext dbContext;
    private readonly BaseRepository<EntityImplementation, int> repository;
    private readonly BaseRepository<EntityDeletableImpl, int> repositoryAuditable;
    private readonly Fixture _fixture = new();
    public BaseRepositoryTests()
    {
        dbContext = TestDbContext.GetInMemoryTestDbContext();
        repository = new BaseRepository<EntityImplementation, int>(dbContext);
        repositoryAuditable = new BaseRepository<EntityDeletableImpl, int>(dbContext);
        _fixture.Customize<EntityImplementation>(
            composer => composer
                                                    .Without(e => e.Id)
                                                    .Without(x => x.LastModified)
                                                    .Without(x => x.LastModifiedBy)
                                                    .Without(x => x.CreatedAt)
                                                    .Without(x => x.CreatedBy)
            );
        _fixture.Customize<EntityDeletableImpl>(
            composer => composer
                                                    .Without(e => e.Id)
                                                    .Without(x => x.LastModified)
                                                    .Without(x => x.LastModifiedBy)
                                                    .Without(x => x.CreatedAt)
                                                    .Without(x => x.CreatedBy)
                                                    .Without(x => x.DeletedAt)
                                                    .Without(x => x.DeletedBy)
            );
    }

    [Fact]
    public async Task AddAsync_NewItem_CreateNewEntryInDb()
    {
        // Arrange
        var name = _fixture.Create<string>();
        var entity = new EntityImplementation()
        {
            Name = name
        };

        // Act
        var result = await repository.AddAsync(entity);

        // Assert
        var savedEntity = await dbContext
            .EntityImpls
            .Where(x => x.Name == name)
            .FirstAsync();
        Assert.NotNull(savedEntity);
        Assert.True(savedEntity.Id > 0);
        Assert.True(result);
        Assert.Equal(name, savedEntity.Name);
    }

    [Fact]
    public async Task DeleteAsync_ExistingItem_RemoveTheItemAndReturnTrue()
    {
        // Arrange
        var entry = GetEntry();
        _ = await repository.AddAsync(entry);

        // Act
        var result = await repository.DeleteAsync(entry);

        // Assert
        var savedEntity = await dbContext
            .EntityImpls
            .Where(x => x.Name == entry.Name)
            .FirstOrDefaultAsync();
        Assert.Null(savedEntity);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteByIdAsync_ExistingItem_RemoveTheItemAndReturnTrue()
    {
        // Arrange
        var entry = GetEntry();
        entry.Id = 1;
        _ = await repository.AddAsync(entry);

        // Act
        var result = await repository.DeleteByIdAsync(entry.Id);

        // Assert
        var savedEntity = await dbContext
            .EntityImpls
            .Where(x => x.Id == entry.Id)
            .FirstOrDefaultAsync();
        Assert.Null(savedEntity);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleAsync_NonExistingItem_ReturnsFalse()
    {
        // Arrange
        var entry = GetEntry();

        // Act
        var result = await repository.DeleteAsync(entry);

        // Assert
        Assert.False(result);

    }

    [Fact]
    public async Task DeleteByIdAsync_NonExistingItem_ReturnsFalse()
    {
        // Act
        var result = await repository.DeleteByIdAsync(_fixture.Create<int>());

        // Assert
        Assert.False(result);

    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSavedData()
    {
        // Arrange
        var entries = _fixture.CreateMany<EntityImplementation>(_fixture.Create<int>()).ToList();
        foreach (var entry in entries)
        {
            await repository.AddAsync(entry);
        }

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entries.Count, result.Count());
        foreach (var entry in entries)
        {
            Assert.Contains(result, r => r.Name == entry.Name && r.Id == entry.Id);
        }
    }

    [Fact]
    public async Task GetAllAsync_AuditableData_ReturnsUndeletedData()
    {
        // Arrange
        var entries = _fixture.CreateMany<EntityDeletableImpl>(5).ToList();
        var twoFirsts = entries.Take(2).ToList();
        foreach (var item in twoFirsts)
        {
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = "Tester";
        }
        foreach (var entry in entries)
        {
            await repositoryAuditable.AddAsync(entry);
        }

        // Act
        var result = await repositoryAuditable.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entries.Count - 2, result.Count());
        foreach (var entry in entries.Skip(2))
        {
            Assert.Contains(result, r => r.Name == entry.Name && r.Id == entry.Id);
        }
        foreach (var entry in twoFirsts)
        {
            Assert.DoesNotContain(result, r => r.Name == entry.Name && r.Id == entry.Id);
        }
    }

    [Fact]
    public async Task GetAllAsync_NotExcludingDeleted_AuditableData_ReturnsAllData()
    {
        // Arrange
        var entries = _fixture.CreateMany<EntityDeletableImpl>(5).ToList();
        var twoFirsts = entries.Take(2).ToList();
        foreach (var item in twoFirsts)
        {
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = "Tester";
        }
        foreach (var entry in entries)
        {
            await repositoryAuditable.AddAsync(entry);
        }

        // Act
        var result = await repositoryAuditable.GetAllAsync(false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entries.Count, result.Count());
        foreach (var entry in entries)
        {
            Assert.Contains(result, r => r.Name == entry.Name && r.Id == entry.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ExistingItem_ReturnsData()
    {
        // Arrange
        var entries = _fixture.CreateMany<EntityDeletableImpl>(5).ToList();
        foreach (var entry in entries)
        {
            _ = await repositoryAuditable.AddAsync(entry);
        }

        // Act
        var result = await repositoryAuditable.GetByIdAsync(entries[0].Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result!.Id, entries[0].Id);
        Assert.Equal(result.Name, entries[0].Name);
        Assert.Equal(result.CreatedBy, entries[0].CreatedBy);
        Assert.Equal(result.CreatedAt, entries[0].CreatedAt);
        Assert.Equal(result.DeletedAt, entries[0].DeletedAt);
        Assert.Equal(result.DeletedBy, entries[0].DeletedBy);
        Assert.Equal(result.LastModified, entries[0].LastModified);
        Assert.Equal(result.LastModifiedBy, entries[0].LastModifiedBy);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingItem_ReturnsNull()
    {
        // Arrange
        var entries = _fixture.CreateMany<EntityDeletableImpl>(5).ToList();
        foreach (var entry in entries)
        {
            _ = await repositoryAuditable.AddAsync(entry);
        }

        // Act
        var result = await repositoryAuditable.GetByIdAsync(10);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingItem_WithDeleteDateSetted_ReturnsNull()
    {
        // Arrange
        var entries = _fixture.CreateMany<EntityDeletableImpl>(5).ToList();
        entries[0].DeletedAt = DateTime.UtcNow;
        foreach (var entry in entries)
        {
            _ = await repositoryAuditable.AddAsync(entry);
        }

        // Act
        var result = await repositoryAuditable.GetByIdAsync(entries[0].Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ExistingEntity_ReturnTrue()
    {
        // Arrange
        const string NAME = "Update data";
        var entity = _fixture.Create<EntityDeletableImpl>();
        var prevName = entity.Name;
        _ = await repositoryAuditable.AddAsync(entity);
        var current = await repositoryAuditable.GetByIdAsync(entity.Id);
        current!.Name = NAME;

        // Act
        var result = await repositoryAuditable.UpdateAsync(current);

        // Assert
        var updated = (await repositoryAuditable.GetByIdAsync(entity.Id))!.Name;
        Assert.True(result);
        Assert.NotEqual(updated, prevName);
    }

    [Fact]
    public async Task UpdateAsync_UnExistingEntity_ReturnFalse()
    {
        // Arrange
        var entity = _fixture.Create<EntityDeletableImpl>();

        // Act
        var result = await repositoryAuditable.UpdateAsync(entity);

        // Assert
        Assert.False(result);
    }

    private EntityImplementation GetEntry() => new()
    {
        Name = _fixture.Create<string>()
    };
}
