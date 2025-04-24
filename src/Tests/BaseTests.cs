using Microsoft.Extensions.Configuration;
using Persistence;

namespace Tests;

public abstract class BaseTests
{
    private readonly DbContextOptions<ApplicationContext> options;

    protected BaseTests()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

        if ("InMemory".Equals(config.GetValue<string>("DatabaseAdapter")))
        {
            optionsBuilder.UseInMemoryDatabase<ApplicationContext>("test");
        }
        else
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("SabadosTechHexagonal"));
        }

        optionsBuilder.LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Information)
                      .EnableSensitiveDataLogging();

        options = optionsBuilder.Options;
    }

    protected ApplicationContext CreateContext(ITenantProvider tenantProvider)
    {
        return new ApplicationContext(options, tenantProvider);
    }

    protected void ClearDatabase(ApplicationContext dataContext)
    {
        dataContext.Configurations.RemoveRange(dataContext.Configurations);
        dataContext.StudentRecords.RemoveRange(dataContext.StudentRecords);
        dataContext.Parents.RemoveRange(dataContext.Parents);
        dataContext.Grades.RemoveRange(dataContext.Grades);
        dataContext.Teachers.RemoveRange(dataContext.Teachers);
        dataContext.Notifications.RemoveRange(dataContext.Notifications);
        dataContext.SaveChanges();
    }
}
