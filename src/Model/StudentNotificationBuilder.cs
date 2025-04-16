namespace Model;

public class StudentNotificationBuilder : NotificationBuilder
{
    private readonly Student student;

    public StudentNotificationBuilder(Student student, string message, DateTime scheduleAt = default) : base(message, scheduleAt)
    {
        this.student = student;
    }

    protected override IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in StudentRecipients(student))
        {
            yield return recipient;
        }
    }
}
