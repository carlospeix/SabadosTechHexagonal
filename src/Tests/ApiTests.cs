using Model;
using Persistence;
using Model.Ports.Driven;

namespace Tests;

public class ApiTests : BaseTests
{
    WebApplicationFactory<Program> app;
    TestTimeProvider testTimeProvider;
    ApplicationContext dataContext;
    ITenantProvider tenantProvider;
    HttpClient client;

    [SetUp]
    public void Setup()
    {
        tenantProvider = new ConstantTenantProvider();
        dataContext = CreateContext(tenantProvider);
        ClearDatabase(dataContext);

        testTimeProvider = new TestTimeProvider(DateTime.UtcNow);

        app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<INotificator>();
                    services.AddSingleton<INotificator, TestNotificator>();
                    services.RemoveAll<ITimeProvider>();
                    services.AddSingleton<ITimeProvider>(testTimeProvider);
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
        await dataContext.SaveChangesAsync();

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
        await dataContext.SaveChangesAsync();

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
        await dataContext.SaveChangesAsync();

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
        await dataContext.SaveChangesAsync();

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

        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        studentRecord.AddParent(parent1);
        studentRecord.AddParent(parent2);
        grade.AddStudent(studentRecord);

        await dataContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade",
            new { GradeId = grade.Id, Message = "Hello grade" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.EqualTo(3));
    }

    #endregion

    #region StudentRecord

    [Test]
    public async Task StudentNotificationHappyPath()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        studentRecord.AddParent(parent1);
        studentRecord.AddParent(parent2);

        await dataContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentRecordId = studentRecord.Id, Message = "Hello student" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.EqualTo(2));
    }

    [Test]
    public async Task StudentNotificationReturnsBadRequestWhenMessageIsEmpty()
    {
        // Arrange
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        await dataContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentRecordId = studentRecord.Id, Message = "" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task StudentNotificationReturnsBadRequestWhenStudentRecordDoesNotExist()
    {
        // Arrange
        const int NON_EXISTENT_STUDENT_RECORD_ID = 100;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentRecordId = NON_EXISTENT_STUDENT_RECORD_ID, Message = "Hello student" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task StudentNotificationReturnsInternalServerErrorWhenExceptionsIsThrown()
    {
        // Arrange
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        await dataContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/student",
            new { StudentRecordId = studentRecord.Id, Message = "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4" });

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
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var parent = new Parent("Mariano", "john@gmail.com", "1111");
        studentRecord.AddParent(parent);

        await dataContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentRecordId = studentRecord.Id, TeacherId = teacher.Id, Message = "Disciplinary notification" });

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
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        await dataContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentRecordId = studentRecord.Id, TeacherId = teacher.Id, Message = "" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsBadRequestWhenStudentRecordDoesNotExist()
    {
        // Arrange
        dataContext.Configurations.Add(new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu"));
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        await dataContext.SaveChangesAsync();
        
        const int NON_EXISTENT_STUDENT_RECORD_ID = 100;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentRecordId = NON_EXISTENT_STUDENT_RECORD_ID, TeacherId = teacher.Id, Message = "Disciplinary notification" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsBadRequestWhenTeacherDoesNotExist()
    {
        // Arrange
        dataContext.Configurations.Add(new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu"));
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        await dataContext.SaveChangesAsync();

        const int NON_EXISTENT_TEACHER_ID = 100;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentRecordId = studentRecord.Id, TeacherId = NON_EXISTENT_TEACHER_ID, Message = "Disciplinary notification" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsInternalServerErrorWhenExceptionsIsThrown()
    {
        // Arrange
        dataContext.Configurations.Add(new Configuration(Configuration.DISCIPLINARY_INBOX, "disciplinary-inbox@school.edu"));
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentRecordId = studentRecord.Id, TeacherId = teacher.Id, Message = "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task DisciplinaryNotificationReturnsInternalServerErrorWhenDisciplinaryInboxIsNotConfigured()
    {
        // Arrange
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        await dataContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/disciplinary",
            new { StudentRecordId = studentRecord.Id, TeacherId = teacher.Id, Message = "Disciplinary notification" });

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
        await dataContext.SaveChangesAsync();

        // Act
        var scheduleAt = testTimeProvider.UtcNow.AddMinutes(30);
        var response = await client.PostAsJsonAsync("/api/v1/notifications/general",
            new { Message = "Hello World", ScheduleAt = scheduleAt.ToString("o") });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.Zero);
    }
}
