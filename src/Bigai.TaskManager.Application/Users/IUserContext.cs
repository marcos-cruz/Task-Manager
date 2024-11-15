namespace Bigai.TaskManager.Application.Users;

public interface IUserContext
{
    CurrentUser? GetCurrentUser();
}
