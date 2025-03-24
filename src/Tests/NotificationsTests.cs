using Model.Ports.Driven;
using Model.UseCases;

namespace Tests;

public class NotificationsTests : BaseTests
{
    ApplicationContext dataContext;

    Secretary secretary;
    IRegistrar registrar;
    TestNotificator notificator;

    [SetUp]
    public void Setup()
    {
        dataContext = CreateContext();
        ClearDatabase(dataContext);

        registrar = dataContext;
        notificator = new();
        secretary = new Secretary(registrar, notificator);
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

        Assert.That(notificator.NotificationsSent, Is.EqualTo(0));
    }

    [Test]
    public void SendNotificationForOneTeacher()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("John Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "Math");
        dataContext.SaveChanges();

        var notificationSent = secretary.SendNotification("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public void SendGlobalNotificationWithJustTeachersRegistered()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.SaveChanges();

        var notifications = new Notifications(registrar, notificator);
        notifications.SendGlobal("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public void SendGlobalNotificationWithTeachersAndParentsRegistered()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", ""));
        dataContext.SaveChanges();

        var notifications = new Notifications(registrar, notificator);
        notifications.SendGlobal("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(2));
    }
}

internal class TestNotificator : INotificator
{
    private int notificationCount;

    public void Send(IEnumerable<Recipient> recipients, string message)
    {
        notificationCount = recipients.Count();
    }

    internal int NotificationsSent()
    {
        return notificationCount;
    }
}
