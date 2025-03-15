namespace Tests;

public class PersistenceTests
{
    ApplicationContext _context;

    [SetUp]
    public void Setup()
    {
        _context = new ApplicationContext();
        _context.ApplyMigrations();
        _context.Configurations.RemoveRange(_context.Configurations);
        _context.Teachers.RemoveRange(_context.Teachers);
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.ChangeTracker.Clear();
        _context.Configurations.RemoveRange(_context.Configurations);
        _context.Teachers.RemoveRange(_context.Teachers);
        _context.SaveChanges();
        _context.Dispose();
    }

    #region Configurations

    [Test]
    public void StartWithNoConfigurations()
    {
        Assert.That(_context.Configurations.Count(), Is.EqualTo(0));
    }

    [Test]
    public void DuplicatedConfigurationNameShouldFail()
    {
        _context.Configurations.Add(new Configuration("OneName", "one value"));
        _context.SaveChanges();
        Assert.Throws<DbUpdateException>(delegate
        {
            _context.Configurations.Add(new Configuration("OneName", "another value"));
            _context.SaveChanges();
        });
    }

    [Test]
    public void ConfigurationValueCanBeModified()
    {
        var config = new Configuration("OneName", "value");
        _context.Configurations.Add(config);
        _context.SaveChanges();
        var id = config.Id;

        _context.Dispose();
        _context = new ApplicationContext();

        config = _context.Configurations.Find(id);
        config?.ChangeValue("new value");
        _context.SaveChanges();

        _context.Dispose();
        _context = new ApplicationContext();

        config = _context.Configurations.Find(id);

        Assert.That(config?.Value, Is.EqualTo("new value"));
    }

    #endregion

    #region Teachers

    [Test]
    public void CanPersistATeacher()
    {
        var teacher = new Teacher("John", "john@school.edu", "1111");
        _context.Teachers.Add(teacher);
        _context.SaveChanges();
        var id = teacher.Id;

        _context.Dispose();
        _context = new ApplicationContext();

        teacher = _context.Teachers.Find(id);

        Assert.That(teacher, Is.Not.Null);
    }

    #endregion
}
