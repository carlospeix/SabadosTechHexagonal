using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Model;
using Persistence;
using Model.Ports.Driven;

namespace Tests;

public class ApiTests : BaseTests
{
    WebApplicationFactory<Program> app;
    ApplicationContext dataContext;
    HttpClient client;

    [SetUp]
    public void Setup()
    {
        dataContext = CreateContext();
        ClearDatabase(dataContext);

        var testDateTimeProvider = new TestTimeProvider(DateTime.UtcNow);

        app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<INotificator>();
                    services.AddSingleton<INotificator, TestNotificator>();
                    services.RemoveAll<ITimeProvider>();
                    services.AddSingleton<ITimeProvider>(testDateTimeProvider);
                });
            });
        client = app.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        dataContext.Dispose();
        client.Dispose();
        app.Dispose();
    }

    #region General notification

    [Test]
    public async Task GeneralNotificationHappyPath()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/general", new { Message = "Hello World" });
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public async Task GeneralNotificationReturnsBadRequestWhenMessageIsEmpty()
    {
        // Arrange

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/general", new { Message = "" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GeneralNotificationReturnsInternalServerErrorWhenExceptionsIsThrown()
    {
        // Arrange

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/general",
            new { Message = "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    #endregion

    #region Grade notification

    [Test]
    public async Task GradeNotificationHappyPath()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade",
            new { GradeId = grade.Id, Message = "Hello grade" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }

    [Test]
    public async Task GradeNotificationReturnsBadRequestWhenMessageIsEmpty()
    {
        // Arrange
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade",
            new { GradeId = grade.Id, Message = "" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GradeNotificationReturnsBadRequestWhenGradeDoesNotExist()
    {
        // Arrange
        const int NON_EXISTENT_GRADE_ID = 100;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade",
            new { GradeId = NON_EXISTENT_GRADE_ID, Message = "Hello grade" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GradeNotificationReturnsInternalServerErrorWhenExceptionsIsThrown()
    {
        // Arrange
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade",
            new { GradeId = grade.Id, Message = "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task GradeNotificationForGradeTeacherAndTwoParents()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");

        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        student.AddParent(parent1);
        student.AddParent(parent2);
        grade.AddStudent(student);

        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade",
            new { GradeId = grade.Id, Message = "Hello grade" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.EqualTo(3));
    }

    #endregion

    #region Student

    [Test]
    public async Task StudentNotificationHappyPath()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        student.AddParent(parent1);
        student.AddParent(parent2);

        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentId = student.Id, Message = "Hello student" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.EqualTo(2));
    }

    [Test]
    public async Task StudentNotificationReturnsBadRequestWhenMessageIsEmpty()
    {
        // Arrange
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentId = student.Id, Message = "" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task StudentNotificationReturnsBadRequestWhenStudentDoesNotExist()
    {
        // Arrange
        const int NON_EXISTENT_STUDENT_ID = 100;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentId = NON_EXISTENT_STUDENT_ID, Message = "Hello student" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task StudentNotificationReturnsInternalServerErrorWhenExceptionsIsThrown()
    {
        // Arrange
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentId = student.Id, Message = "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    #endregion

    #region Disciplinary

    [Test]
    public async Task DisciplinaryNotificationHappyPath()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        var config = dataContext.Configurations.Add(
            new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var parent = new Parent("Mariano", "john@gmail.com", "1111");
        student.AddParent(parent);

        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentId = student.Id, TeacherId = teacher.Id, Message = "Disciplinary notification" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        // A notification is sent to parent, another to the teacher and another to the special address
        Assert.That(notificator.NotificationsSent, Is.EqualTo(3));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsBadRequestWhenMessageIsEmpty()
    {
        // Arrange
        dataContext.Configurations.Add(new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu"));
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentId = student.Id, TeacherId = teacher.Id, Message = "" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsBadRequestWhenStudentDoesNotExist()
    {
        // Arrange
        dataContext.Configurations.Add(new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu"));
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        dataContext.SaveChanges();
        
        const int NON_EXISTENT_STUDENT_ID = 100;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentId = NON_EXISTENT_STUDENT_ID, TeacherId = teacher.Id, Message = "Disciplinary notification" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsBadRequestWhenTeacherDoesNotExist()
    {
        // Arrange
        dataContext.Configurations.Add(new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu"));
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        dataContext.SaveChanges();

        const int NON_EXISTENT_TEACHER_ID = 100;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentId = student.Id, TeacherId = NON_EXISTENT_TEACHER_ID, Message = "Disciplinary notification" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsInternalServerErrorWhenExceptionsIsThrown()
    {
        // Arrange
        dataContext.Configurations.Add(new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu"));
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentId = student.Id, TeacherId = teacher.Id, Message = "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsInternalServerErrorWhenDisciplinaryInboxIsNotConfigured()
    {
        // Arrange
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentId = student.Id, TeacherId = teacher.Id, Message = "Disciplinary notification" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    #endregion

    [Test]
    public async Task FutureNotificationHappyPath()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        dataContext.SaveChanges();

        // Act
        var scheduleAt = DateTime.UtcNow.AddMinutes(30);
        var response = await client.PostAsJsonAsync("/api/v1/notifications/general",
            new { Message = "Hello World", ScheduleAt = scheduleAt.ToString("o") });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.Zero);
    }

    [Test, Ignore("Prepared for the video")]
    public async Task FutureNotificationSchedule30MinutesAheadAndSent()
    {
        // Arrange
        var testDateTimeProvider = (TestTimeProvider)app.Services.GetRequiredService<ITimeProvider>();
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        dataContext.Parents.Add(new Parent("Mariano", "john@gmail.com", "1111"));
        dataContext.SaveChanges();

        // Act
        var scheduleAt = testDateTimeProvider.UtcNow.AddMinutes(30);
        var response = await client.PostAsJsonAsync("/api/v1/notifications/general",
            new { Message = "Hello World", ScheduleAt = scheduleAt.ToString("o") });
        
        testDateTimeProvider.TravelBy(TimeSpan.FromMinutes(35));
        // hacer algo

        // Assert
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }
}
