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

        app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<INotificator>();
                    services.AddSingleton<INotificator, TestNotificator>();
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
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade", new { GradeId = grade.Id, Message = "" });

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
        // Arrange
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade",
            new { GradeId = grade.Id, Message = "Please Throw! 8756D35F-B8AE-4018-BFCF-2148ADDA1EF4" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}
