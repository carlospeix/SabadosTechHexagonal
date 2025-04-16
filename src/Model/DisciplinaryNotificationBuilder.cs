using Model.Ports.Driven;

namespace Model;

public class DisciplinaryNotificationBuilder : NotificationBuilder
{
    private readonly IRegistrar registrar;
    private readonly Student student;
    private readonly Teacher teacher;

    public DisciplinaryNotificationBuilder(IRegistrar registrar, Student student, Teacher teacher, string message, DateTime scheduleAt = default) : base(message, scheduleAt)
    {
        this.registrar = registrar;
        this.student = student;
        this.teacher = teacher;
    }

    public override Notification Build()
    {
        return new Notification(GetRecipients(), Message, ScheduleAt);
    }

    private IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in StudentRecipients(student))
        {
            yield return recipient;
        }
        yield return teacher.GetRecipient();
        yield return DisciplinaryInboxRecipient();
    }

    private Recipient DisciplinaryInboxRecipient()
    {
        var config = registrar.Configurations
            .FirstOrDefault(c => c.Name == Configuration.DISCIPLINARY_INBOX);

        if (config is null)
        {
            throw new InvalidOperationException("Disciplinary inbox configuration not found");
        }

        return new Recipient("Disciplinary ionbox", config.Value, "");
    }
}
