namespace Model;

public class GradeNotificationBuilder : NotificationBuilder
{
    private readonly Grade grade;

    public GradeNotificationBuilder(Grade grade, string message, DateTime scheduleAt = default) : base(message, scheduleAt)
    {
        this.grade = grade;
    }

    public override Notification Build()
    {
        return new Notification(GetRecipients(), Message, ScheduleAt);
    }

    private IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in GradeRecipients(grade))
        {
            yield return recipient;
        }
    }
}
