using System.Net;

using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Services;

namespace Bigai.TaskManager.Infrastructure.Projects.Services;

public sealed class BussinessNotificationsHandler : IBussinessNotificationsHandler
{
    private List<BussinessNotification> _notifications;

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public BussinessNotificationsHandler()
    {
        _notifications = new List<BussinessNotification>();
    }

    public IReadOnlyCollection<BussinessNotification> GetNotifications()
    {
        return _notifications;
    }

    public bool HasNotification()
    {
        return _notifications.Any();
    }

    public void NotifyError(BussinessNotification notification)
    {
        _notifications.Add(notification);
    }

    public void NotifyError(string code, string message)
    {
        NotifyError(new BussinessNotification(code, message));
    }

    public void ClearNotification()
    {
        _notifications = new List<BussinessNotification>();
    }
}