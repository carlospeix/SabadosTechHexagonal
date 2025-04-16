using Model.Ports.Driven;

namespace Model;

public class GeneralNotificationBuilder : NotificationBuilder
{
    private readonly IRegistrar registrar;

    public GeneralNotificationBuilder(IRegistrar registrar, string message, DateTime scheduleAt = default) : base(message, scheduleAt)
    {
        this.registrar = registrar;
    }

    protected override IEnumerable<Recipient> GetRecipients()
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
            yield return parent.GetRecipient();
        }
    }
}
