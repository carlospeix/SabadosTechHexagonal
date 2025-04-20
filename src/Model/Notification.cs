using Model.Ports.Driven;

namespace Model;

public class Notification
{
    public int Id { get; private set; }

    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }
    public DateTime? SentAt { get; private set; }

    public IReadOnlyCollection<Recipient> Recipients => recipients.ToList().AsReadOnly();
    private readonly HashSet<Recipient> recipients = [];

    private Notification() { }

    public Notification(IEnumerable<Recipient> recipients, string message, DateTime scheduleAt)
    {
        this.recipients = [.. recipients];
        Message = message;
        ScheduleAt = scheduleAt;
    }

    public async Task SendIfItIsTime(INotificator notificator, ITimeProvider timeProvider)
    {
        if (ShouldSendAt(timeProvider.UtcNow))
        {
            await notificator.Send(Recipients, Message);
            MarkAsSentAt(timeProvider.UtcNow);
        }
    }

    private bool ShouldSendAt(DateTime utcNow)
    {
        return utcNow >= ScheduleAt;
    }

    private void MarkAsSentAt(DateTime utcNow)
    {
        SentAt = utcNow;
    }
}
