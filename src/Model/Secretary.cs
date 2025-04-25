using Model.Ports.Driven;

namespace Model;

public class Secretary(IRegistrar registrar, INotificator notificator, ITimeProvider timeProvider)
{
    private readonly IRegistrar registrar = registrar;
    private readonly INotificator notificator = notificator;
    private readonly ITimeProvider timeProvider = timeProvider;

    public async Task SendGeneralNotification(string message, DateTime scheduleAt)
    {
        var builder = new GeneralNotificationBuilder(registrar, message, BringToNowPastScheduleDates(scheduleAt));

        await SendNotification(builder);
    }

    public async Task SendGradeNotification(Grade grade, string message)
    {
        var builder = new GradeNotificationBuilder(grade, message);

        await SendNotification(builder);
    }

    public async Task SendStudentNotification(StudentRecord student, string message)
    {
        var builder = new StudentNotificationBuilder(student, message);

        await SendNotification(builder);
    }

    public async Task SendDisciplinaryNotification(StudentRecord student, Teacher teacher, string message)
    {
        var builder = new DisciplinaryNotificationBuilder(student, teacher, message, registrar);

        await SendNotification(builder);
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

    private async Task SendNotification(NotificationBuilder builder)
    {
        var notification = await builder.Build();

        registrar.AddNotification(notification);

        await notification.SendIfItIsTime(notificator, timeProvider);
    }

    private DateTime BringToNowPastScheduleDates(DateTime scheduleAt)
    {
        return scheduleAt == default ? timeProvider.UtcNow : scheduleAt;
    }
}
