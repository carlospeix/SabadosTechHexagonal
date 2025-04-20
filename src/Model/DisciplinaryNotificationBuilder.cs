namespace Model;

public class DisciplinaryNotificationBuilder : NotificationBuilder
{
    private readonly Student student;
    private readonly Teacher teacher;
    private readonly Action<Notification> specialRecipientsDelegate;

    public DisciplinaryNotificationBuilder(Student student, Teacher teacher, string message, Action<Notification> specialRecipientsDelegate) : base(message, default)
    {
        this.student = student;
        this.teacher = teacher;
        this.specialRecipientsDelegate = specialRecipientsDelegate;
    }

    protected override void AddRecipientsTo(Notification notification)
    {
        AddStudentRecipientsTo(student, notification);
        notification.AddRecipient(teacher.Name, teacher.Email, teacher.Phone);
        specialRecipientsDelegate.Invoke(notification);
    }
}
