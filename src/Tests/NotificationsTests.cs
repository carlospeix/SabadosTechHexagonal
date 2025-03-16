
namespace Tests;

public class NotificationsTests : BaseTests
{
    ApplicationContext dataContext;

    [SetUp]
    public void Setup()
    {
        dataContext = CreateContext();
        ClearDatabase(dataContext);
    }

    [TearDown]
    public void TearDown()
    {
        dataContext.ChangeTracker.Clear();
        ClearDatabase(dataContext);
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

        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("John Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "Math");
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