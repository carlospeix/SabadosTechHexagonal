
namespace Model;

public class Secretary
{
    private INotificationSender notificationSender;

    public Secretary(INotificationSender notificationSender)
    {
        this.notificationSender = notificationSender;
    }

    public bool SendNotification(string message)
    {
        return true;
    }
}
