namespace Bigai.TaskManager.Application.Users;

public record CurrentUser(int UserId, IEnumerable<string> Roles)
{
    public bool IsInRole(string role) => Roles.Contains(role);
}