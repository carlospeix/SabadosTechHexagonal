namespace Model;

public class StudentNotificationBuilder : NotificationBuilder
{
    private readonly StudentRecord studentRecord;

    public StudentNotificationBuilder(StudentRecord studentRecord, string message) : base(message, default)
    {
        this.studentRecord = studentRecord;
    }

    protected override Task AddRecipientsTo(Notification notification)
    {
        AddStudentRecipientsTo(studentRecord, notification);

        return Task.CompletedTask;
    }
}
