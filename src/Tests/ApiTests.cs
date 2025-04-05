using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Model;

namespace Tests;

public class ApiTests : BaseTests
{
    [Test]
    public async Task NotificationHappyPath()
    {
        // Arrange
        var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => {});
            });
        
        // Act
        var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/notifications/global", new { Message = "Hello World" });
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task GradeNotificationHappyPath()
    {
        // Arrange
        var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => { });
            });

        var dataContext = CreateContext();
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("Jophn Doe", "john@school.edu", "")).Entity;
        grade.AddSubject(teacher, "History");
        dataContext.SaveChanges();
        var gradeId = grade.Id;

        // Act
        var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/notifications/grade", new { GradeId = gradeId, Message = "Hello grade" });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
}
