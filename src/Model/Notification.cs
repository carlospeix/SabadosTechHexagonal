namespace Model;

public class Notification
{
    public int Id { get; private set; }

    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }

    public IReadOnlyCollection<Recipient> Recipients => recipients.ToList().AsReadOnly();
    private readonly HashSet<Recipient> recipients = [];

    private Notification() { }

    public Notification(IEnumerable<Recipient> recipients, string message, DateTime scheduleAt)
    {
        this.recipients = [.. recipients];
        Message = message;
        ScheduleAt = scheduleAt;
    }

    public bool ShouldSendNow()
    {
        return DateTime.UtcNow >= ScheduleAt;
    }
}
