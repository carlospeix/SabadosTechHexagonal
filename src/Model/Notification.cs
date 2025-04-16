namespace Model;

public class Notification
{
    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }

    public IReadOnlyCollection<Recipient> Recipients => recipients.ToList().AsReadOnly();
    private readonly HashSet<Recipient> recipients = [];

    public Notification(string message, DateTime scheduleAt, IEnumerable<Recipient> recipients)
    {
        Message = message;
        ScheduleAt = scheduleAt;
        this.recipients = [.. recipients];
    }

    public bool ShouldSendNow()
    {
        return DateTime.UtcNow >= ScheduleAt;
    }
}
