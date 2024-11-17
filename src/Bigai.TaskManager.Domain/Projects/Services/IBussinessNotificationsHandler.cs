using System.Net;

using Bigai.TaskManager.Domain.Projects.Notifications;

namespace Bigai.TaskManager.Domain.Projects.Services;

public interface IBussinessNotificationsHandler
{
    HttpStatusCode StatusCode { get; set; }

    bool HasNotification();

    IReadOnlyCollection<BussinessNotification> GetNotifications();

    void NotifyError(BussinessNotification notification);

    void NotifyError(string code, string message);

    void ClearNotification();
}