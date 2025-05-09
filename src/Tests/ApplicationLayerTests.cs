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
    public async Task SendNoNotificationWhenNoRecipients()
    {
        await notifications.SendGeneral("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(0));
    }

    [Test]
    public async Task SendGeneralNotificationWithJustTeachersRegistered()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        await dataContext.SaveChangesAsync(CancellationToken.None);

        await notifications.SendGeneral("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public async Task SendGeneralNotificationWithTeachersAndParentsRegistered()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", ""));
        await dataContext.SaveChangesAsync(CancellationToken.None);

        await notifications.SendGeneral("Hello World");

        Assert.That(notificator.NotificationsSent, Is.EqualTo(2));
    }

    [Test]
    public void ShouldThrowIfMessageIsEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(async () => await notifications.SendGeneral(""));
    }

    [Test]
    public async Task FutureNotificationHappyPath()
    {
        // Arrange
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        await dataContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var scheduleAt = testTimeProvider.UtcNow.AddMinutes(30);
        await notifications.SendGeneral("Hello World", scheduleAt);

        // Assert
        Assert.That(notificator.NotificationsSent, Is.Zero);
    }

    [Test]
    public async Task FutureNotificationSchedule30MinutesAheadAndSent()
    {
        // Arrange
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        await dataContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var scheduleAt = testTimeProvider.UtcNow.AddMinutes(30);
        await notifications.SendGeneral("Hello World", scheduleAt);
        Assert.That(notificator.NotificationsSent, Is.EqualTo(0));
        notificator.Reset();

        // Simulate the passage of time and act
        testTimeProvider.TravelBy(TimeSpan.FromMinutes(35));
        await notifications.SendPendingNotifications(CancellationToken.None);

        // Assert
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public async Task FutureNotificationSchedule30MinutesAheadSentAndNotResent()
    {
        // Arrange
        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        await dataContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var scheduleAt = testTimeProvider.UtcNow.AddMinutes(30);
        await notifications.SendGeneral("Hello World", scheduleAt);
        Assert.That(notificator.NotificationsSent, Is.Zero);

        // Simulate the passage of time and act
        testTimeProvider.TravelBy(TimeSpan.FromMinutes(35));
        await notifications.SendPendingNotifications(CancellationToken.None);

        // Assert
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
        notificator.Reset();

        // Send again, should not send
        await notifications.SendPendingNotifications(CancellationToken.None);

        // Assert
        Assert.That(notificator.NotificationsSent, Is.Zero);
    }
}
