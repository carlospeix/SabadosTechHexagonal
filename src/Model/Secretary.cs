using Model.Ports.Driven;

namespace Model;

public class Secretary
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;

    public Secretary(IRegistrar registrar, INotificator notificator)
    {
        this.registrar = registrar;
        this.notificator = notificator;
    }

    public void SendNotification(Notification notification)
    {
        var recipients = notification.GetRecipients();
        notificator.Send(recipients, notification.Message);
    }
}
