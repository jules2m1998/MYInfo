namespace MYInfo.API.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    public string GetUserIdentifier() => httpContextAccessor
        .HttpContext?
        .User
        .FindFirst(ClaimTypes.NameIdentifier)?
        .Value ?? string.Empty;
}
