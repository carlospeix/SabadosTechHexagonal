using Model.Ports.Driven;

namespace Model;

public class Notification : TenantEntity
{
    public int Id { get; private set; }

    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }
    public DateTime? SentAt { get; private set; }

    public IReadOnlyCollection<Recipient> Recipients => recipients.ToList().AsReadOnly();
    private readonly HashSet<Recipient> recipients = [];

    private Notification() { }

    public Notification(string message, DateTime scheduleAt)
    {
        Message = message;
        ScheduleAt = scheduleAt;
    }

    public void AddRecipient(string name, string email, string phone)
    {
        recipients.Add(new Recipient(this, name, email, phone));
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
