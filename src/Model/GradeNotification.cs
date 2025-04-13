namespace Model;

public class GradeNotification : Notification
{
    private readonly Grade grade;

    public GradeNotification(Grade grade, string message)
    {
        this.grade = grade;
        Message = message;
    }

    public override IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in GradeRecipients(grade))
        {
            yield return recipient;
        }
    }
}
