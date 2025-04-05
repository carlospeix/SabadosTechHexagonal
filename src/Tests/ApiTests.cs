using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests;

public class ApiTests
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
}
