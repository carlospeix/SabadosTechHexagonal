using Model;
using Persistence;

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
        if (dataContext.Database.IsInMemory())
        {
            Assert.Ignore("In-memory database does not enforce unique constraints");
        }

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

    #region Teachers, Grades, Subjects, Parents, Students

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
    public void CanPersistAParent()
    {
        var parent = new Parent("Mariano", "john@gmail.com", "1111");
        dataContext.Parents.Add(parent);
        dataContext.SaveChanges();
        var id = parent.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        parent = dataContext.Parents.Find(id);

        Assert.That(parent, Is.Not.Null);
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

    [Test]
    public void CanNotDeleteATeacherAssociatedToASubject()
    {
        if (dataContext.Database.IsInMemory())
        {
            Assert.Ignore("In-memory database does not enforce referential integrity.");
        }

        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("John", "john@school.edu", "1111")).Entity;
        _ = grade.AddSubject(teacher, "Math");

        dataContext.SaveChanges();
        var id = teacher.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        Assert.Throws<DbUpdateException>(() =>
        {
            dataContext.Teachers.RemoveRange(dataContext.Teachers);
            dataContext.SaveChanges();
        });
    }

    [Test]
    public void DeletingGradeDoesNotDeleteTeacher()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var teacher = dataContext.Teachers.Add(new Teacher("John", "john@school.edu", "1111")).Entity;
        _ = grade.AddSubject(teacher, "Math");

        dataContext.SaveChanges();

        dataContext.Dispose();
        dataContext = CreateContext();

        dataContext.Grades.RemoveRange(dataContext.Grades);
        dataContext.SaveChanges();

        Assert.That(dataContext.Teachers.Count(), Is.EqualTo(1));
    }

    [Test]
    public void CanPersistAStudent()
    {
        var student = new Student("Student 1");
        dataContext.Students.Add(student);
        dataContext.SaveChanges();
        var id = student.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        student = dataContext.Students.Find(id);

        Assert.That(student, Is.Not.Null);
    }

    [Test]
    public void CanAddAStudentToAGrade()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var student = new Student("Student 1");
        grade.AddStudent(student);

        dataContext.SaveChanges();
        var id = grade.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        grade = dataContext.Grades.Find(id);
        Assert.That(grade?.Students.Count, Is.EqualTo(1));

        student = grade.Students.First();
        Assert.That(student.Grade, Is.Not.Null);
    }

    [Test]
    public void CanAddAParentToAStudent()
    {
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var parent = new Parent("Mariano", "john@gmail.com", "1111");
        student.AddParent(parent);

        dataContext.SaveChanges();
        var id = student.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        student = dataContext.Students.Find(id);
        Assert.That(student, Is.Not.Null);

        Assert.That(student.Parents, Has.Count.EqualTo(1));
    }

    [Test]
    public void CanAddMoreThanOneParentToAStudent()
    {
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        student.AddParent(parent1);
        student.AddParent(parent2);

        dataContext.SaveChanges();
        var id = student.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        student = dataContext.Students.Find(id);
        Assert.That(student, Is.Not.Null);
        parent1 = student.Parents.First();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(student?.Parents.Count, Is.EqualTo(2));
            Assert.That(parent1?.Students.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void DeletesRelationshipWhenParentIsDeleted()
    {
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        student.AddParent(parent1);
        student.AddParent(parent2);

        dataContext.SaveChanges();
        var id = student.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        student = dataContext.Students.Find(id);
        Assert.That(student, Is.Not.Null);
        Assert.That(student.Parents, Has.Count.EqualTo(2));

        var parent = student.Parents.Last();
        dataContext.Parents.Remove(parent);
        dataContext.SaveChanges();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(student.Parents, Has.Count.EqualTo(1));
            Assert.That(student.CaregivingRelationships, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void StoresARelationshipNameWhenProvided()
    {
        var student = dataContext.Students.Add(new Student("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Marie", "aut_marie@gmail.com", "222");
        student.AddParent(parent1);
        student.AddParent(parent2, "Aunt");

        dataContext.SaveChanges();
        var id = student.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        student = dataContext.Students.Find(id);
        parent1 = dataContext.Parents.First(p => p.Name == "Mariano");
        parent2 = dataContext.Parents.First(p => p.Name == "Marie");
        Assert.That(student, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(student.CaregivingRelationships.First(r => r.Parent == parent1).Name, Is.EqualTo("Parent"));
            Assert.That(student.CaregivingRelationships.First(r => r.Parent == parent2).Name, Is.EqualTo("Aunt"));
        }
    }
    #endregion

    #region Notifications

    [Test]
    public void CanPersistANotification()
    {
        var recipients = new List<Recipient> 
        {
            new("Teacher 1", "teacher-1@school.edu", "111"),
            new("Teacher 2", "teacher-2@school.edu", "222")
        };
        var notification = dataContext.Notifications.Add(new Notification(recipients, "Hello world", DateTime.UtcNow)).Entity;

        dataContext.SaveChanges();
        var id = notification.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        notification = dataContext.Notifications.Find(id);

        Assert.That(notification, Is.Not.Null);
        Assert.That(notification.Recipients, Has.Count.EqualTo(2));
    }

    [Test]
    public void PersistSentOnTime()
    {
        var recipients = new List<Recipient>
        {
            new("Teacher 1", "teacher-1@school.edu", "111"),
            new("Teacher 2", "teacher-2@school.edu", "222")
        };
        var notification = dataContext.Notifications.Add(new Notification(recipients, "Hello world", DateTime.UtcNow)).Entity;
        dataContext.SaveChanges();

        var id = notification.Id;

        dataContext.Dispose();
        dataContext = CreateContext();

        notification = dataContext.Notifications.Find(id);
        Assert.That(notification?.SentOn, Is.Null);

        var timeProvider = new TestTimeProvider(new DateTime(2025, 10, 10));
        notification?.SendIfItIsTime(new TestNotificator(), timeProvider);
        
        dataContext.SaveChanges();

        dataContext.Dispose();
        dataContext = CreateContext();

        notification = dataContext.Notifications.Find(id);

        Assert.That(notification?.SentOn, Is.EqualTo(timeProvider.UtcNow));
    }

    #endregion

}
