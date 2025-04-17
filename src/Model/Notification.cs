using Model.Ports.Driven;

namespace Model;

public class Notification
{
    public int Id { get; private set; }

    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }
    public DateTime? SentOn { get; private set; }

    public IReadOnlyCollection<Recipient> Recipients => recipients.ToList().AsReadOnly();
    private readonly HashSet<Recipient> recipients = [];

    private Notification() { }

    public Notification(IEnumerable<Recipient> recipients, string message, DateTime scheduleAt)
    {
        this.recipients = [.. recipients];
        Message = message;
        ScheduleAt = scheduleAt;
    }

    public void SendIfItIsTime(INotificator notificator, ITimeProvider timeProvider)
    {
        if (ShouldSend(timeProvider.UtcNow))
        {
            notificator.Send(Recipients, Message);
            MarkAsSentOn(timeProvider.UtcNow);
        }
    }

    private void MarkAsSentOn(DateTime utcNow)
    {
        SentOn = utcNow;
    }

    private bool ShouldSend(DateTime utcNow)
    {
        return utcNow >= ScheduleAt;
    }
}
