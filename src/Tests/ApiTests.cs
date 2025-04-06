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
    public async Task GradeNotificationHappyPath()
    {
        // Arrange
        var notificator = (TestNotificator)app.Services.GetRequiredService<INotificator>();

        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade", new { GradeId = grade.Id, Message = "Hello grade" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(notificator.NotificationsSent, Is.EqualTo(1));
    }
}
