using Model.Ports.Driven;

namespace Model;

public class Secretary
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;

    public Secretary(IRegistrar registrar, INotificator notificator)
    {
        this.registrar = registrar;
        this.notificator = notificator;
    }

    public void SendNotification(Notification notification)
    {
        registrar.AddNotification(notification);

        if (notification.ShouldSendNow())
        {
            notificator.Send(notification.Recipients, notification.Message);
        }
    }

    public void SendNotificationsScheduledAtOrBefore(DateTime utcNow)
    {
        foreach (var notification in registrar.Notifications.Where(n => n.ScheduleAt <= utcNow))
        {
            notificator.Send(notification.Recipients, notification.Message);
        }
    }
}
