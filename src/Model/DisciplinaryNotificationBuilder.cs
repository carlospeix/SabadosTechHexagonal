using Model.Ports.Driven;

namespace Model;

public class DisciplinaryNotificationBuilder : NotificationBuilder
{
    private readonly StudentRecord studentRecord;
    private readonly Teacher teacher;
    private readonly IRegistrar registrar;

    public DisciplinaryNotificationBuilder(StudentRecord studentRecord, Teacher teacher, string message, IRegistrar registrar) : base(message, default)
    {
        this.studentRecord = studentRecord;
        this.teacher = teacher;
        this.registrar = registrar;
    }

    protected override async Task AddRecipientsTo(Notification notification)
    {
        var disciplinaryInboxConfig = await registrar.RequiredConfigurationByName(
            Configuration.DISCIPLINARY_INBOX, "Disciplinary inbox configuration not valid.");

        AddStudentRecipientsTo(studentRecord, notification);
        notification.AddRecipient(teacher.Name, teacher.Email, teacher.Phone);
        notification.AddRecipient("Disciplinary inbox", disciplinaryInboxConfig.Value, string.Empty);
    }
}
