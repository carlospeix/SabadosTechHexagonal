using Model;
using Application;
using Persistence;

namespace Tests;

public class ApplicationLayerTests : BaseTests
{
    ApplicationContext dataContext;

    TestNotificator notificator;
    TestTimeProvider testTimeProvider;
    ITenantProvider tenantProvider;
    Notifications notifications;

    [SetUp]
    public void Setup()
    {
        tenantProvider = new ConstantTenantProvider();
        dataContext = CreateContext(tenantProvider);
        ClearDatabase(dataContext);

        var registrar = new Registrar(dataContext);
        var unitOfWork = new UnitOfWork(dataContext);
        notificator = new();
        testTimeProvider = new TestTimeProvider(DateTime.UtcNow);
        notifications = new Notifications(unitOfWork, registrar, notificator, testTimeProvider);
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
        var scheduleAt = testTimeProvider.UtcNow.AddMinutes(30);
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
        notificator.Reset();

        // Simulate the passage of time and act
        testTimeProvider.TravelBy(TimeSpan.FromMinutes(35));
        notifications.SendPendingNotifications();

        // Assert
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public void FutureNotificationSchedule30MinutesAheadSentAndNotResent()
    {
        // Arrange
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        dataContext.SaveChanges();

        // Act
        var scheduleAt = testTimeProvider.UtcNow.AddMinutes(30);
        notifications.SendGeneral("Hello World", scheduleAt);
        Assert.That(notificator.NotificationsSent, Is.Zero);

        // Simulate the passage of time and act
        testTimeProvider.TravelBy(TimeSpan.FromMinutes(35));
        notifications.SendPendingNotifications();

        // Assert
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
        notificator.Reset();

        // Send again, should not send
        notifications.SendPendingNotifications();

        // Assert
        Assert.That(notificator.NotificationsSent, Is.Zero);
    }
}
