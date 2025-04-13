using Model.Ports.Driven;

namespace Model;

public class GeneralNotification : Notification
{
    private readonly IRegistrar registrar;

    public GeneralNotification(IRegistrar registrar, string message)
    {
        this.registrar = registrar;
        Message = message;
    }

    public override  IEnumerable<Recipient> GetRecipients()
    {
        foreach (var grade in registrar.Grades)
        {
            foreach (var recipient in GradeRecipients(grade))
            {
                yield return recipient;
            }
        }
        foreach (var parent in registrar.Parents)
        {
            yield return new Recipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
