namespace Bigai.TaskManager.Domain.Projects.Notifications;

public class BussinessNotification
{
    public string Code { get; init; }

    public string Message { get; init; }

    public BussinessNotification()
    {
        Code = string.Empty;
        Message = string.Empty;
    }

    public BussinessNotification(string code, string message)
    {
        Code = code;
        Message = message;
    }
}