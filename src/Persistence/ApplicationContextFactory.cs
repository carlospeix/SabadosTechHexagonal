using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Persistence;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        var connectionString = config.GetConnectionString("SabadosTechHexagonal");
        if (connectionString == null)
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=SabadosTechHexagonalData.mdf;Integrated Security=True";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer(connectionString);

        optionsBuilder.LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Information)
                      .EnableSensitiveDataLogging();
        
        return new ApplicationContext(optionsBuilder.Options);
    }
}
