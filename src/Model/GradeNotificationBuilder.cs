namespace Model;

public class GradeNotificationBuilder : NotificationBuilder
{
    private readonly Grade grade;

    public GradeNotificationBuilder(Grade grade, string message) : base(message, default)
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
