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

    protected override IEnumerable<Recipient> GetRecipients()

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
        var config = registrar.ConfigurationByName(Configuration.DISCIPLINARY_INBOX) ??
            throw new InvalidOperationException("Disciplinary inbox configuration not found");

        return new Recipient("Disciplinary ionbox", config.Value, "");
    }
}