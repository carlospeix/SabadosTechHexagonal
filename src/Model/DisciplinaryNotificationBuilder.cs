namespace Model;

public class DisciplinaryNotificationBuilder : NotificationBuilder
{
    private readonly StudentRecord studentRecord;
    private readonly Teacher teacher;
    private readonly Action<Notification> specialRecipientsDelegate;

    public DisciplinaryNotificationBuilder(StudentRecord studentRecord, Teacher teacher, string message, Action<Notification> specialRecipientsDelegate) : base(message, default)
    {
        this.studentRecord = studentRecord;
        this.teacher = teacher;
        this.specialRecipientsDelegate = specialRecipientsDelegate;
    }

    protected override Task AddRecipientsTo(Notification notification)
    {
        AddStudentRecipientsTo(studentRecord, notification);
        notification.AddRecipient(teacher.Name, teacher.Email, teacher.Phone);
        specialRecipientsDelegate.Invoke(notification);

        return Task.CompletedTask;
    }
}
