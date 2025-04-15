using Model.Ports.Driven;

namespace Model;

public class Secretary
{
    private readonly INotificator notificator;

    public Secretary(INotificator notificator)
    {
        this.notificator = notificator;
    }

    public void SendNotification(Notification notification)
    {
        var recipients = notification.GetRecipients();

        if (notification.ShouldSendNow())
        {
            notificator.Send(recipients, notification.Message);
        }
    }
}
