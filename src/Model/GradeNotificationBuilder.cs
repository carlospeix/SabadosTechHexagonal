namespace Model;

public class GradeNotificationBuilder : NotificationBuilder
{
    private readonly Grade grade;

    public GradeNotificationBuilder(Grade grade, string message)
    {
        this.grade = grade;
        Message = message;
    }

    public override Notification Build()
    {
        return new Notification(Message, ScheduleAt, GetRecipients());
    }

    private IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in GradeRecipients(grade))
        {
            yield return recipient;
        }
    }
}
