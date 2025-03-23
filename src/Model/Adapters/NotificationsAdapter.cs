using Model.Ports;

namespace Model.Adapters;

public class NotificationsAdapter : INotifications
{
    private IRegistrar registrar;
    private INotificationSender notificationSender;

    public NotificationsAdapter(IRegistrar registrar, INotificationSender notificationSender)
    {
        this.registrar = registrar;
        this.notificationSender = notificationSender;
    }

    public void SendGlobal(string message)
    {
        var secretary = new Secretary(registrar, notificationSender);
        secretary.SendNotification(message);
    }
}
