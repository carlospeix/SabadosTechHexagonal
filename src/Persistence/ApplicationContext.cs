using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;

namespace Persistence;

public class ApplicationContext : DbContext, IRegistrar
{
    private string connectionString;

    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    IQueryable<Teacher> IRegistrar.Teachers => Teachers;

    public ApplicationContext()
    {
        InitializeDatabase();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(connectionString)
            .LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Information)
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

        modelBuilder.Entity<Teacher>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.Property(t => t.Email).HasMaxLength(100);
            x.Property(t => t.Phone).HasMaxLength(100);
        });
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

    private void InitializeDatabase()
    {
        // Por ahora dejamos esto aquí, pero debería ir en un archivo o servicio de configuración 
        string relativePath = @"SabadosTech\SabadosTechHexagonalData.mdf";
        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string fullPath = Path.Combine(userFolder, relativePath) ?? throw new InvalidDataException("Error getting the database path");

        connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={fullPath};Integrated Security=True";

        if (File.Exists(fullPath))
        {
            return;
        }

        var directoryPath = Path.GetDirectoryName(fullPath);
        if (directoryPath != null)
        {
            Directory.CreateDirectory(directoryPath);
        }

        try
        {
            string tempConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
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
