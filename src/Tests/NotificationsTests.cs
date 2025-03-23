using Model.Adapters;
using Model.Ports;

namespace Tests;

public class NotificationsTests : BaseTests
{
    Secretary secretary;
    ApplicationContext dataContext;
    TestNotificationSender notificationSender;

    [SetUp]
    public void Setup()
    {
        dataContext = CreateContext();
        ClearDatabase(dataContext);

        notificationSender = new();
        secretary = new Secretary(dataContext, notificationSender);
    }

    [TearDown]
    public void TearDown()
    {
        dataContext.Dispose();
    }

    [Test]
    public void SendNotification()
    {
        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSent, Is.True);
    }

    [Test]
    public void SendNoNotificationWhenNoRecipients()
    {
        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSender.NotificationsSent, Is.EqualTo(0));
    }

    [Test]
    public void SendNotificationForOneTeacher()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("John Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "Math");
        dataContext.SaveChanges();

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificationSender.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public void SendGlobalNotificationWithJustTeachersRegistered()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Mariano", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.SaveChanges();

        var notifications = new NotificationsAdapter(CreateContext(), notificationSender);
        notifications.SendGlobal("Hello World");

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
