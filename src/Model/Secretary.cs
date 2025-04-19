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

        notification.SendIfItIsTime(notificator, timeProvider);
    }

    public void SendPendingNotifications()
    {
        var pendingNotifications = registrar.FilteredNotifications(
            n => n.ScheduleAt <= timeProvider.UtcNow && n.SentAt == null);

        foreach (var notification in pendingNotifications)
        {
            notification.SendIfItIsTime(notificator, timeProvider);
        }
    }
}
