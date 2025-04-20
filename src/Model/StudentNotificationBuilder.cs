namespace Model;

public class StudentNotificationBuilder : NotificationBuilder
{
    private readonly Student student;

    public StudentNotificationBuilder(Student student, string message) : base(message, default)
    {
        this.student = student;
    }

    protected override void AddRecipientsTo(Notification notification)
    {
        AddStudentRecipientsTo(student, notification);
    }
}
