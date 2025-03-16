namespace Tests;

public class PersistenceTests : BaseTests
{
    ApplicationContext dataContext;

    [SetUp]
    public void Setup()
    {
        dataContext = CreateContext();
        ClearDatabase(dataContext);
    }

    [TearDown]
    public void TearDown()
    {
        dataContext.ChangeTracker.Clear();
        ClearDatabase(dataContext);
        dataContext.Dispose();
    }

    #region Configurations

    [Test]
    public void StartWithNoConfigurations()
    {
        Assert.That(dataContext.Configurations.Count(), Is.EqualTo(0));
    }

    [Test]
    public void DuplicatedConfigurationNameShouldFail()
    {
        dataContext.Configurations.Add(new Configuration("OneName", "one value"));
        dataContext.SaveChanges();
        Assert.Throws<DbUpdateException>(delegate
        {
            dataContext.Configurations.Add(new Configuration("OneName", "another value"));
            dataContext.SaveChanges();
        });
    }

    [Test]
    public void ConfigurationValueCanBeModified()
    {
        var config = new Configuration("OneName", "value");
        dataContext.Configurations.Add(config);
        dataContext.SaveChanges();
        var id = config.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        config = dataContext.Configurations.Find(id);
        config?.ChangeValue("new value");
        dataContext.SaveChanges();

        dataContext.Dispose();
        dataContext = CreateContext();

        config = dataContext.Configurations.Find(id);

        Assert.That(config?.Value, Is.EqualTo("new value"));
    }

    #endregion

    #region Teachers, Grades, Subjects

    [Test]
    public void CanPersistATeacher()
    {
        var teacher = new Teacher("John", "john@school.edu", "1111");
        dataContext.Teachers.Add(teacher);
        dataContext.SaveChanges();
        var id = teacher.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        teacher = dataContext.Teachers.Find(id);

        Assert.That(teacher, Is.Not.Null);
    }

    [Test]
    public void CanPersistAGrade()
    {
        var grade = new Grade("10th grade");
        dataContext.Grades.Add(grade);
        dataContext.SaveChanges();
        var id = grade.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        grade = dataContext.Grades.Find(id);

        Assert.That(grade, Is.Not.Null);
    }

    [Test]
    public void CanPersistASubject()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("John", "john@school.edu", "1111")).Entity;
        _ = grade.AddSubject(teacher, "Math");

        dataContext.SaveChanges();
        var id = grade.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        grade = dataContext.Grades.Find(id);

        Assert.That(grade?.Subjects.Count, Is.EqualTo(1));
    }

    #endregion
}
