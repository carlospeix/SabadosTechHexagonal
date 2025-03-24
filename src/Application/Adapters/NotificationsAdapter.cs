using Model.Ports.Driven;
using Model.Ports.Driver;

namespace Application.Adapters;

public class NotificationsAdapter : INotifications
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;

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
