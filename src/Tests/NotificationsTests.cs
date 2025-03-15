
namespace Tests;

public class NotificationsTests
{
    ApplicationContext dataContext;

    [SetUp]
    public void Setup()
    {
        dataContext = new ApplicationContext();
        dataContext.ApplyMigrations();
        dataContext.Configurations.RemoveRange(dataContext.Configurations);
        dataContext.Teachers.RemoveRange(dataContext.Teachers);
        dataContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        dataContext.ChangeTracker.Clear();
        dataContext.Configurations.RemoveRange(dataContext.Configurations);
        dataContext.Teachers.RemoveRange(dataContext.Teachers);
        dataContext.SaveChanges();
        dataContext.Dispose();
    }

    [Test]
    public void SendNotification()
    {
        var secretary = new Secretary(dataContext, new TestNotificationSender());

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSent, Is.True);
    }

    [Test]
    public void SendNoNotificationWhenNoRecipients()
    {
        var notificationSender = new TestNotificationSender();
        var secretary = new Secretary(dataContext, notificationSender);

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSender.NotificationsSent, Is.EqualTo(0));
    }

    [Test]
    public void SendNotificationForOneTeacher()
    {
        var notificationSender = new TestNotificationSender();
        var secretary = new Secretary(dataContext, notificationSender);

        dataContext.Teachers.Add(new Teacher("John Doe", "john@school.edu", ""));
        dataContext.SaveChanges();

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