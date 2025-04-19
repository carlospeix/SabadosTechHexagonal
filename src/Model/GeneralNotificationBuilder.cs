using Model.Ports.Driven;

namespace Model;

public class GeneralNotificationBuilder : NotificationBuilder
{
    private readonly IRegistrar registrar;

    public GeneralNotificationBuilder(IRegistrar registrar, string message, DateTime scheduleAt) : base(message, scheduleAt)
    {
        this.registrar = registrar;
    }

    protected override IEnumerable<Recipient> GetRecipients()
    {
        foreach (var grade in registrar.AllGrades)
        {
            foreach (var recipient in GradeRecipients(grade))
            {
                yield return recipient;
            }
        }
        foreach (var parent in registrar.AllParents)
        {
            yield return parent.GetRecipient();
        }
    }
}
