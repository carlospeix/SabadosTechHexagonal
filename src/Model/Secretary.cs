using Model.Ports.Driven;

namespace Model;

public class Secretary(IRegistrar registrar, INotificator notificator, ITimeProvider timeProvider)
{
    private readonly IRegistrar registrar = registrar;
    private readonly INotificator notificator = notificator;
    private readonly ITimeProvider timeProvider = timeProvider;

    public async Task SendNotification(Notification notification)
    {
        registrar.AddNotification(notification);

        await notification.SendIfItIsTime(notificator, timeProvider);
    }

    public async Task SendPendingNotifications(CancellationToken cancellationToken)
    {
        var pendingNotifications = registrar.FilteredNotifications(
            n => n.ScheduleAt <= timeProvider.UtcNow && n.SentAt == null);

        await foreach (var notification in pendingNotifications)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await notification.SendIfItIsTime(notificator, timeProvider);
        }
    }
}
