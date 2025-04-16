using Model.Ports.Driven;

namespace Model;

public class Secretary
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;
    private readonly ITimeProvider timeProvider;

    public Secretary(IRegistrar registrar, INotificator notificator, ITimeProvider timeProvider)
    {
        this.registrar = registrar;
        this.notificator = notificator;
        this.timeProvider = timeProvider;
    }

    public void SendNotification(Notification notification)
    {
        registrar.AddNotification(notification);

        if (notification.ShouldSendNow())
        {
            notificator.Send(notification.Recipients, notification.Message);
        }
    }

    public void SendScheduledNotifications()
    {
        foreach (var notification in registrar.Notifications.Where(n => n.ScheduleAt <= timeProvider.UtcNow))
        {
            notificator.Send(notification.Recipients, notification.Message);
        }
    }
}
