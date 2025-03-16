using Microsoft.EntityFrameworkCore;
using Model;

namespace Persistence;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options), IRegistrar
{
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Grade> Grades { get; set; }

    IQueryable<Teacher> IRegistrar.Teachers => Teachers;
    IQueryable<Grade> IRegistrar.Grades => Grades;

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

        modelBuilder.Entity<Grade>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.HasMany(t => t.Subjects).WithOne(t => t.Grade).IsRequired();
            x.Navigation(t => t.Subjects).AutoInclude();
        });

        modelBuilder.Entity<Subject>(x =>
        {
            x.ToTable("Subjects");
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.HasOne(t => t.Teacher).WithMany().IsRequired().OnDelete(DeleteBehavior.NoAction);
        });
    }
}
