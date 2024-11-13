using Bigai.TaskManager.Domain.Projects.Notifications;

namespace Bigai.TaskManager.Domain.Projects.Services;

public interface IBussinessNotificationsHandler
{
    bool HasNotification();

    IReadOnlyCollection<BussinessNotification> GetNotifications();

    void NotifyError(BussinessNotification notification);

    void NotifyError(string code, string message);

    void ClearNotification();
}