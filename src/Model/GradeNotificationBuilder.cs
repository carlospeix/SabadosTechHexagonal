namespace Model;

public class GradeNotificationBuilder : NotificationBuilder
{
    private readonly Grade grade;

    public GradeNotificationBuilder(Grade grade, string message, DateTime scheduleAt = default) : base(message, scheduleAt)
    {
        this.grade = grade;
    }

    protected override IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in GradeRecipients(grade))
        {
            yield return recipient;
        }
    }
}
