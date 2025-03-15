
namespace Tests;

public class NotificationsTests
{
    [Test]
    public void SendNotification()
    {
        var secretary = new Secretary(null);

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSent, Is.True);
    }

    [Test]
    public void SendNoNotificationWhenNoRecipients()
    {
        var notificationSender = new TestNotificationSender();
        var secretary = new Secretary(notificationSender);

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSender.NotificationsSent, Is.EqualTo(0));
    }
}

internal class TestNotificationSender : INotificationSender
{
    public TestNotificationSender()
    {
    }

    internal int NotificationsSent()
    {
        return 0;
    }
}