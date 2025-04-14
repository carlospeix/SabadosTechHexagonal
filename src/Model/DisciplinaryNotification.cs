using Model.Ports.Driven;

namespace Model;

public class DisciplinaryNotification : Notification
{
    private readonly IRegistrar registrar;
    private readonly Student student;
    private readonly Teacher teacher;

    public DisciplinaryNotification(IRegistrar registrar, Student student, Teacher teacher, string message)
    {
        this.registrar = registrar;
        this.student = student;
        this.teacher = teacher;
        Message = message;
    }

    public override IEnumerable<Recipient> GetRecipients()
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
