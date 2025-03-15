namespace Tests;

public class NotificationsTests
{
    [Test]
    public void SendNotification()
    {
        var secretary = new Secretary();

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSent, Is.True);
    }
}
