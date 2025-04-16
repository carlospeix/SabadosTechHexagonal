using Application;
using Model;
using Model.Ports.Driven;
using Persistence;

namespace Tests;

public class ApplicationLayerTests : BaseTests
{
    ApplicationContext dataContext;

    IRegistrar registrar;
    TestNotificator notificator;
    TestTimeProvider testTimeProvider;
    Notifications notifications;

    [SetUp]
    public void Setup()
    {
        dataContext = CreateContext();
        ClearDatabase(dataContext);

        registrar = dataContext;
        notificator = new();
        testTimeProvider = new TestTimeProvider(DateTime.UtcNow);
        notifications = new Notifications(registrar, notificator);
    }

    [TearDown]
    public void TearDown()
    {
        dataContext.Dispose();
    }

    [Test]
    public void SendNoNotificationWhenNoRecipients()
    {
        notifications.SendGeneral("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(0));
    }

    [Test]
    public void SendGeneralNotificationWithJustTeachersRegistered()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.SaveChanges();

        notifications.SendGeneral("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public void SendGeneralNotificationWithTeachersAndParentsRegistered()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", ""));
        dataContext.SaveChanges();

        notifications.SendGeneral("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(2));
    }

    [Test]
    public void ShouldThrowIfMessageIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => notifications.SendGeneral(""));
    }

    [Test]
    public void FutureNotificationHappyPath()
    {
        // Arrange
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        dataContext.SaveChanges();

        // Act
        var scheduleAt = DateTime.UtcNow.AddMinutes(30);
        notifications.SendGeneral("Hello World", scheduleAt);

        // Assert
        Assert.That(notificator.NotificationsSent, Is.Zero);
    }

    [Test]
    public void FutureNotificationSchedule30MinutesAheadAndSent()
    {
        // Arrange
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        dataContext.SaveChanges();

        // Act
        var scheduleAt = testTimeProvider.UtcNow.AddMinutes(30);
        notifications.SendGeneral("Hello World", scheduleAt);
        Assert.That(notificator.NotificationsSent, Is.EqualTo(0));

        // Simulate the passage of time and act
        testTimeProvider.TravelBy(TimeSpan.FromMinutes(35));

        notifications.SendNotificationsScheduledAtOrBefore(testTimeProvider.UtcNow);

        // Assert
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }
}
