using MYInfo.Domain.Models;
using MYInfo.Domain.ValueObjects;

namespace MYInfo.Domain.Repositories;

public interface IUserMetadataRepository : IBaseRepository<UserMetaData, UserMetaDataId>
{
    Task<UserMetaData?> GetByUserIdAsync(string userId);
}
