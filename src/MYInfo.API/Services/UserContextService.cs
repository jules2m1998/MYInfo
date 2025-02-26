using MYInfo.Domain.Services;

namespace MYInfo.API.Services;

public class UserContextService : IUserContextService
{
    public string GetUserIdentifier()
    {
        return Guid.CreateVersion7().ToString();
    }
}
