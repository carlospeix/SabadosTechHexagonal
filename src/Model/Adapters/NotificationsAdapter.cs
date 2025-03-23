using Model.Ports;

namespace Model.Adapters;

public class NotificationsAdapter : INotifications
{
    private IRegistrar registrar;
    private INotificator notificator;

    public NotificationsAdapter(IRegistrar registrar, INotificator notificator)
    {
        this.registrar = registrar;
        this.notificator = notificator;
    }

    public void SendGlobal(string message)
    {
        var secretary = new Secretary(registrar, notificator);
        secretary.SendNotification(message);
    }
}
