
namespace Model;

public class Secretary
{
    private readonly IRegistrar registrar;
    private INotificationSender notificationSender;

    public Secretary(IRegistrar registrar, INotificationSender notificationSender)
    {
        this.registrar = registrar;
        this.notificationSender = notificationSender;
    }

    public bool SendNotification(string message)
    {
        foreach (var recipient in Recipients())
        {
            notificationSender.Send(recipient, message);
        }

        return true;
    }

    private IList<Recipient> Recipients()
    {
        return registrar.Teachers.Select(t => new Recipient(t.Name, t.Email, t.Phone)).ToList();
    }
}
