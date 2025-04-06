using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Model;
using Persistence;

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
                builder.ConfigureServices(services => { });
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
    public async Task NotificationHappyPath()
    {
        // Arrange
        
        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/general", new { Message = "Hello World" });
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task GradeNotificationHappyPath()
    {
        // Arrange
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.SaveChanges();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade", new { GradeId = grade.Id, Message = "Hello grade" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
}
