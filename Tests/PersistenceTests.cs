namespace Tests;

public class PersistenceTests
{
    ApplicationContext _context;

    [SetUp]
    public void Setup()
    {
        _context = new ApplicationContext();
        _context.Configurations.RemoveRange(_context.Configurations);
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.ChangeTracker.Clear();
        _context.Configurations.RemoveRange(_context.Configurations);
        _context.SaveChanges();
        _context.Dispose();
    }

    [Test]
    public void IniciaConCeroConfiguraciones()
    {
        Assert.That(_context.Configurations.Count(), Is.EqualTo(0));
    }

    [Test]
    public void NombreDeConfiguracionDuplicadoDeberíaFallar()
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
    public void PuedeModificarValorDeConfiguracion()
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
}
