using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace Bigai.TaskManager.Application.Users;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser? GetCurrentUser()
    {
        var user = (_httpContextAccessor?.HttpContext?.User) ?? throw new InvalidOperationException("User context is no present");

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return null;
        }

        var nameId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role)!.Select(c => c.Value);
        var userId = user.FindFirst(c => c.Type == "UserId")?.Value;
        int id = 0;

        int.TryParse(userId, out id);

        return new CurrentUser(id, roles);
    }
}