using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;

namespace Mapping;

public class ApplicationContext : DbContext
{
    private string connectionString;

    public ApplicationContext()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // Por ahora dejamos esto aquí, pero debería ir en un archivo o servicio de configuración 
        string relativePath = @"SabadosTech\SabadosTechHexagonalData.mdf";
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string fullPath = Path.Combine(userFolder, relativePath);

        string tempConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
        connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullPath};Integrated Security=True";

        if (!File.Exists(fullPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            try
            {
                using (var connection = new SqlConnection(tempConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        $"CREATE DATABASE [{Path.GetFileNameWithoutExtension(fullPath)}] " +
                        $"ON (NAME = N'{Path.GetFileNameWithoutExtension(fullPath)}', " +
                        $"FILENAME = '{fullPath}')", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error creating database: {ex.Message}");
            }
        }
    }

    public void ApplyMigrations()
    {
        try
        {
            Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations: {ex.Message}");
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(connectionString)
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

        modelBuilder.Entity<Configuration>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.Property(t => t.Value).HasMaxLength(500);
            x.HasIndex(t => t.Name).IsUnique();
        });
        
    }

    public DbSet<Configuration> Configurations { get; set; }
}
