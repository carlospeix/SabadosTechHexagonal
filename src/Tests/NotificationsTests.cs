
namespace Tests;

public class NotificationsTests
{
    [Test]
    public void SendNotification()
    {
        var secretary = new Secretary(new TestNotificationSender());

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSent, Is.True);
    }

    [Test, Ignore("Until recipient collection is calculated according the database.")]
    public void SendNoNotificationWhenNoRecipients()
    {
        var notificationSender = new TestNotificationSender();
        var secretary = new Secretary(notificationSender);

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSender.NotificationsSent, Is.EqualTo(0));
    }

    [Test]
    public void SendNotificationWhenOneRecipients()
    {
        var notificationSender = new TestNotificationSender();
        var secretary = new Secretary(notificationSender);

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSender.NotificationsSent, Is.EqualTo(1));
    }
}

internal class TestNotificationSender : INotificationSender
{
    private int notificationCount = 0;

    public TestNotificationSender()
    {
    }

    public void Send(Recipient recipient, string message)
    {
        notificationCount++;
    }

    internal int NotificationsSent()
    {
        return notificationCount;
    }
}