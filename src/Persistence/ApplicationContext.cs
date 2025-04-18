using System.Reflection;
using System.Reflection.Emit;
using Model;
using Model.Ports.Driven;

namespace Persistence;

public class ApplicationContext : DbContext, IRegistrar
{
    private const string TENANT_ID_FIELD_NAME = "TenantId";
    private readonly int tenantId;
    private readonly Action<TenantEntity, int> tenantIdSetter;

    public ApplicationContext(DbContextOptions<ApplicationContext> options, ITenantProvider? tenantProvider = default) : base(options)
    {
        tenantId = tenantProvider?.GetTenantId() ?? 0;
        
        var tenantIdFieldInfo = typeof(TenantEntity).GetField(TENANT_ID_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new InvalidOperationException($"Field '{TENANT_ID_FIELD_NAME}' not found on type '{nameof(TenantEntity)}'.");
        
        tenantIdSetter = CreateSetter<TenantEntity, int>(tenantIdFieldInfo);
    }

    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    IQueryable<Teacher> IRegistrar.Teachers => Teachers;
    IQueryable<Grade> IRegistrar.Grades => Grades;
    IQueryable<Parent> IRegistrar.Parents => Parents;
    IQueryable<Student> IRegistrar.Students => Students;
    IQueryable<Configuration> IRegistrar.Configurations => Configurations;
    IQueryable<Notification> IRegistrar.Notifications => Notifications;

    void IRegistrar.AddNotification(Notification notification)
    {
        Notifications.Add(notification);
    }

    void IRegistrar.SaveChanges()
    {
        SaveChanges();
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

            AddTenancySupport(x);
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
            x.HasMany(t => t.Students).WithOne(t => t.Grade).IsRequired(false);
            x.Navigation(t => t.Subjects).AutoInclude();
            x.Navigation(t => t.Students).AutoInclude();
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
        });

        modelBuilder.Entity<Student>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Name).HasMaxLength(100);
            x.HasMany(t => t.Parents).WithMany(t => t.Students).UsingEntity<CaregivingRelationship>().ToTable("CaregivingRelationships");
            x.Navigation(t => t.CaregivingRelationships).AutoInclude();
            x.Navigation(t => t.Parents).AutoInclude();
        });

        modelBuilder.Entity<Notification>(x =>
        {
            x.HasKey(t => t.Id);
            x.Property(t => t.Message).HasMaxLength(1000);
            x.Property(t => t.ScheduleAt);
            x.Property(t => t.SentAt);
            x.HasMany(t => t.Recipients).WithOne(t => t.Notification).IsRequired();
            x.Navigation(t => t.Recipients).AutoInclude();
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

    private void AddTenancySupport(EntityTypeBuilder<Configuration> x)
    {
        x.Property(TENANT_ID_FIELD_NAME).IsRequired();
        x.HasIndex(TENANT_ID_FIELD_NAME);
        x.HasQueryFilter(t => EF.Property<int>(t, TENANT_ID_FIELD_NAME) == tenantId);
    }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                tenantIdSetter(entry.Entity, tenantId);
            }
        }

        return base.SaveChanges();
    }

    private Action<S, T> CreateSetter<S, T>(FieldInfo field)
    {
        string methodName = $"{field?.ReflectedType?.FullName}.set_{field?.Name}";
        
        DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[] { typeof(S), typeof(T) }, true);
        ILGenerator gen = setterMethod.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Stfld, field);
        gen.Emit(OpCodes.Ret);

        return (Action<S, T>)setterMethod.CreateDelegate(typeof(Action<S, T>));
    }
}
