
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
        foreach (var recipient in Recipients())
        {
            notificationSender.Send(recipient, message);
        }

        return true;
    }

    private IList<Recipient> Recipients()
    {
        return new List<Recipient>() {
            new Recipient()
        };
    }
}
