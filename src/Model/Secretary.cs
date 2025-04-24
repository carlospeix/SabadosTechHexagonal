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

        await SendNotification(builder.Build());
    }

    public async Task SendGradeNotification(Grade grade, string message)
    {
        var builder = new GradeNotificationBuilder(grade, message);

        await SendNotification(builder.Build());
    }

    public async Task SendStudentNotification(StudentRecord student, string message)
    {
        var builder = new StudentNotificationBuilder(student, message);

        await SendNotification(builder.Build());
    }

    public async Task SendDisciplinaryNotification(StudentRecord student, Teacher teacher, string message)
    {
        var disciplinaryInboxConfig = await registrar.RequiredConfigurationByName(
            Configuration.DISCIPLINARY_INBOX, "Disciplinary inbox configuration not valid.");

        var builder = new DisciplinaryNotificationBuilder(student, teacher, message,
            notification => notification.AddRecipient("Disciplinary inbox", disciplinaryInboxConfig.Value, string.Empty));

        await SendNotification(builder.Build());
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

    private async Task SendNotification(Notification notification)
    {
        registrar.AddNotification(notification);

        await notification.SendIfItIsTime(notificator, timeProvider);
    }

    private DateTime BringToNowPastScheduleDates(DateTime scheduleAt)
    {
        return scheduleAt == default ? timeProvider.UtcNow : scheduleAt;
    }
}
