using System.Reflection;
using System.Reflection.Emit;
using Model;

namespace Persistence;

public class ApplicationContext : DbContext
{
    private const string TENANT_ID_FIELD_NAME = "TenantId";
    private readonly int tenantId;
    private readonly Action<TenantEntity, int> tenantIdSetter;

    public ApplicationContext(DbContextOptions<ApplicationContext> options, ITenantProvider tenantProvider) : base(options)
    {
        tenantId = tenantProvider.GetTenantId();
        
        var tenantIdFieldInfo = typeof(TenantEntity).GetField(TENANT_ID_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new InvalidOperationException($"Field '{TENANT_ID_FIELD_NAME}' not found on type '{nameof(TenantEntity)}'.");
        
        tenantIdSetter = CreateSetter<TenantEntity, int>(tenantIdFieldInfo);
    }

    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<StudentRecord> StudentRecords { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

        modelBuilder.Entity<Configuration>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.Property(t => t.Value).HasMaxLength(500);
            x.HasIndex(t => t.Name).IsUnique();

            AddTenancySupport(x);
        });

        modelBuilder.Entity<Teacher>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.Property(t => t.Email).HasMaxLength(100);
            x.Property(t => t.Phone).HasMaxLength(100);

            AddTenancySupport(x);
        });

        modelBuilder.Entity<Grade>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.StudentName).HasMaxLength(100);
            x.HasMany(t => t.Subjects).WithOne(t => t.Grade).IsRequired();
            x.HasMany(t => t.StudentRecords).WithOne(t => t.CurrentGrade).IsRequired(false);
            x.Navigation(t => t.Subjects).AutoInclude();
            x.Navigation(t => t.StudentRecords).AutoInclude();

            AddTenancySupport(x);
        });

        modelBuilder.Entity<Subject>(x =>
        {
            x.ToTable("Subjects");
            x.HasKey("Id");
            x.Property("Id").UseIdentityColumn();
            x.Property(t => t.Name).HasMaxLength(100);
            x.HasOne(t => t.Teacher).WithMany().IsRequired().OnDelete(DeleteBehavior.NoAction);
            x.Navigation(t => t.Teacher).AutoInclude();
        });

        modelBuilder.Entity<Parent>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.Property(t => t.Email).HasMaxLength(100);
            x.Property(t => t.Phone).HasMaxLength(100);

            AddTenancySupport(x);
        });

        modelBuilder.Entity<StudentRecord>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.StudentName).HasMaxLength(100);
            x.HasMany(t => t.Parents).WithMany().UsingEntity<CaregivingRelationship>().ToTable("CaregivingRelationships");

            AddTenancySupport(x);
        });

        modelBuilder.Entity<Notification>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Message).HasMaxLength(1000);
            x.Property(t => t.ScheduleAt);
            x.Property(t => t.SentAt);
            x.HasMany(t => t.Recipients).WithOne(t => t.Notification).IsRequired();
            x.Navigation(t => t.Recipients).AutoInclude();

            AddTenancySupport(x);
        });

        modelBuilder.Entity<Recipient>(x =>
        {
            x.ToTable("Recipients");
            x.HasKey("Id");
            x.Property("Id").UseIdentityColumn();
            x.Property(t => t.Name).HasMaxLength(100);
            x.Property(t => t.Email).HasMaxLength(100);
            x.Property(t => t.Phone).HasMaxLength(100);
        });
    }

    public override int SaveChanges()
    {
        AddTenantIdToAddedEntities(CancellationToken.None);

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTenantIdToAddedEntities(cancellationToken);

        return base.SaveChangesAsync(cancellationToken);    
    }

    private void AddTenantIdToAddedEntities(CancellationToken cancellationToken)
    {
        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entry.State == EntityState.Added)
            {
                tenantIdSetter(entry.Entity, tenantId);
            }
        }
    }

    private void AddTenancySupport<T>(EntityTypeBuilder<T> x) where T : TenantEntity
    {
        x.Property(TENANT_ID_FIELD_NAME).IsRequired();
        x.HasIndex(TENANT_ID_FIELD_NAME);
        x.HasQueryFilter(t => EF.Property<int>(t, TENANT_ID_FIELD_NAME) == tenantId);
    }

    private Action<S, T> CreateSetter<S, T>(FieldInfo field)
    {
        ArgumentNullException.ThrowIfNull(field);

        string methodName = $"{field.ReflectedType!.FullName}.set_{field.Name}";
        
        var setterMethod = new DynamicMethod(methodName, null, [typeof(S), typeof(T)], true);
        var gen = setterMethod.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Stfld, field);
        gen.Emit(OpCodes.Ret);

        return (Action<S, T>)setterMethod.CreateDelegate(typeof(Action<S, T>));
    }
}
