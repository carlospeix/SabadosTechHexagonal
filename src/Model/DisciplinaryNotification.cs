

using Model.Ports.Driven;

namespace Model;

public class DisciplinaryNotification : Notification
{
    private IRegistrar registrar;
    private Student student;
    private Teacher teacher;

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
            .FirstOrDefault(c => c.Name == "DISCIPLINARY_INBOX");
        return new Recipient("Disciplinary ionbox", config.Value, "");
    }
}
