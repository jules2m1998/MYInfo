namespace MYInfo.Infrastructure.Persistence.Data.Repositories;

public class UserMetadataRepository(IDbContext dbContext) : BaseRepository<UserMetaData, UserMetaDataId>(dbContext), IUserMetadataRepository
{
    public async Task<UserMetaData?> GetByUserIdAsync(string userId)
    {
        return await Entity.Where(x => x.CreatedBy == userId).FirstOrDefaultAsync();
    }
}
