using Model.Ports.Driven;

namespace Model;

public class Secretary(IRegistrar registrar, INotificator notificator, ITimeProvider timeProvider)
{
    private readonly IRegistrar registrar = registrar;
    private readonly INotificator notificator = notificator;
    private readonly ITimeProvider timeProvider = timeProvider;

    public void SendNotification(Notification notification)
    {
        registrar.AddNotification(notification);

        notification.SendIfItIsTime(notificator, timeProvider);
    }

    public async Task SendPendingNotifications(CancellationToken cancellationToken)
    {
        var pendingNotifications = registrar.FilteredNotifications(
            n => n.ScheduleAt <= timeProvider.UtcNow && n.SentAt == null);

        await foreach (var notification in pendingNotifications)
        {
            cancellationToken.ThrowIfCancellationRequested();

            notification.SendIfItIsTime(notificator, timeProvider);
        }
    }
}
