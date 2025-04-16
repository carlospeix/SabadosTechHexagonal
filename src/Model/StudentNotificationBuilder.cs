namespace Model;

public class StudentNotificationBuilder : NotificationBuilder
{
    private readonly Student student;

    public StudentNotificationBuilder(Student student, string message, DateTime scheduleAt = default)
    {
        this.student = student;
        ScheduleAt = scheduleAt;
        Message = message;
    }

    public override Notification Build()
    {
        return new Notification(Message, ScheduleAt, GetRecipients());
    }

    private IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in StudentRecipients(student))
        {
            yield return recipient;
        }
    }
}
