using Microsoft.Extensions.Configuration;

namespace Tests;

public abstract class BaseTests
{
    private readonly IConfiguration config;

    protected BaseTests()
    {
        config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
    }

    protected ApplicationContext CreateContext()
    {
        return new ApplicationContext(config);
    }

    protected void ClearDatabase(ApplicationContext dataContext)
    {
        dataContext.Configurations.RemoveRange(dataContext.Configurations);
        dataContext.Grades.RemoveRange(dataContext.Grades);
        dataContext.Teachers.RemoveRange(dataContext.Teachers);
        dataContext.SaveChanges();
    }
}
