using Model.Ports.Driven;

namespace Model;

public class DisciplinaryNotificationBuilder : NotificationBuilder
{
    private readonly IRegistrar registrar;
    private readonly Student student;
    private readonly Teacher teacher;

    public DisciplinaryNotificationBuilder(IRegistrar registrar, Student student, Teacher teacher, string message) : base(message, default)
    {
        this.registrar = registrar;
        this.student = student;
        this.teacher = teacher;
    }

    protected override void AddRecipientsTo(Notification notification)
    {
        AddStudentRecipientsTo(student, notification);
        notification.AddRecipient(teacher.Name, teacher.Email, teacher.Phone);

        var config = registrar.ConfigurationByName(Configuration.DISCIPLINARY_INBOX) ??
            throw new InvalidOperationException("Disciplinary inbox configuration not found");

        notification.AddRecipient("Disciplinary inbox", config.Value, "");
    }
}
