using Model;
using Persistence;
using System.Reflection;

namespace Tests;

public class PersistenceTests : BaseTests
{
    ApplicationContext dataContext;
    ITenantProvider tenantProvider;

    [SetUp]
    public void Setup()
    {
        tenantProvider = new ConstantTenantProvider();
        dataContext = CreateContext(tenantProvider);
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
        dataContext = CreateContext(tenantProvider);

        config = dataContext.Configurations.Find(id);
        config?.ChangeValue("new value");
        dataContext.SaveChanges();

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        config = dataContext.Configurations.Find(id);

        Assert.That(config?.Value, Is.EqualTo("new value"));
    }

    #endregion

    #region Teachers, Grades, Subjects, Parents, StudentRecords

    [Test]
    public void CanPersistATeacher()
    {
        var teacher = new Teacher("John", "john@school.edu", "1111");
        dataContext.Teachers.Add(teacher);
        dataContext.SaveChanges();
        var id = teacher.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

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
        dataContext = CreateContext(tenantProvider);

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
        dataContext = CreateContext(tenantProvider);

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
        dataContext = CreateContext(tenantProvider);

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
        dataContext = CreateContext(tenantProvider);

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
        dataContext = CreateContext(tenantProvider);

        dataContext.Grades.RemoveRange(dataContext.Grades);
        dataContext.SaveChanges();

        Assert.That(dataContext.Teachers.Count(), Is.EqualTo(1));
    }

    [Test]
    public void CanPersistAStudentRecord()
    {
        var studentRecord = new StudentRecord("Student 1");
        dataContext.StudentRecords.Add(studentRecord);
        dataContext.SaveChanges();
        var id = studentRecord.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        studentRecord = dataContext.StudentRecords.Find(id);

        Assert.That(studentRecord, Is.Not.Null);
    }

    [Test]
    public void CanAddAStudentRecordToAGrade()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var studentRecord = new StudentRecord("Student 1");
        grade.AddStudent(studentRecord);

        dataContext.SaveChanges();
        var id = grade.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        grade = dataContext.Grades.Find(id);
        Assert.That(grade?.StudentRecords.Count, Is.EqualTo(1));

        studentRecord = grade.StudentRecords.First();
        Assert.That(studentRecord.CurrentGrade, Is.Not.Null);
    }

    [Test]
    public void AddingTheSameStudentToAGradeHasNoEffect()
    {
        var grade = dataContext.Grades.Add(new Grade("10th grade")).Entity;
        var studentRecord = new StudentRecord("Student 1");
        grade.AddStudent(studentRecord);

        dataContext.SaveChanges();
        var gradeId = grade.Id;
        var studentRecordId = studentRecord.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        grade = dataContext.Grades.Find(gradeId);
        Assert.That(grade?.StudentRecords.Count, Is.EqualTo(1));

        studentRecord = dataContext.StudentRecords.Find(studentRecordId);
        Assert.That(studentRecord, Is.Not.Null);

        grade.AddStudent(studentRecord);
        dataContext.SaveChanges();

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        grade = dataContext.Grades.Find(gradeId);
        Assert.That(grade?.StudentRecords.Count, Is.EqualTo(1));
    }

    [Test]
    public void CanAddAParentToAStudentRecord()
    {
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var parent = new Parent("Mariano", "john@gmail.com", "1111");
        studentRecord.AddParent(parent);

        dataContext.SaveChanges();
        var id = studentRecord.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        studentRecord = dataContext.StudentRecords
            .Include(s => s.CaregivingRelationships).ThenInclude(cgr => cgr.Parent)
            .Where(s => s.Id == id).FirstOrDefault();

        Assert.That(studentRecord, Is.Not.Null);

        Assert.That(studentRecord.CaregivingRelationships, Has.Count.EqualTo(1));
    }

    [Test]
    public void CanAddMoreThanOneParentToAStudent()
    {
        var student = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        student.AddParent(parent1);
        student.AddParent(parent2);

        dataContext.SaveChanges();
        var id = student.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        student = dataContext.StudentRecords
            .Include(s => s.CaregivingRelationships).ThenInclude(cgr => cgr.Parent)
            .Where(s => s.Id == id).FirstOrDefault();

        Assert.That(student, Is.Not.Null);
        parent1 = student.Parents.First();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(student?.CaregivingRelationships, Has.Count.EqualTo(2));
        }
    }

    [Test]
    public void DeletesRelationshipWhenParentIsDeleted()
    {
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Carlos", "carlos@gmail.com", "222");
        studentRecord.AddParent(parent1);
        studentRecord.AddParent(parent2);

        dataContext.SaveChanges();
        var id = studentRecord.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        studentRecord = dataContext.StudentRecords
            .Include(s => s.CaregivingRelationships).ThenInclude(cgr => cgr.Parent)
            .Where(s => s.Id == id).FirstOrDefault();

        Assert.That(studentRecord, Is.Not.Null);
        Assert.That(studentRecord.CaregivingRelationships, Has.Count.EqualTo(2));

        var parent = studentRecord.Parents.Last();
        dataContext.Parents.Remove(parent);
        dataContext.SaveChanges();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(studentRecord.Parents, Has.Count.EqualTo(1));
            Assert.That(studentRecord.CaregivingRelationships, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void StoresARelationshipNameWhenProvided()
    {
        var studentRecord = dataContext.StudentRecords.Add(new StudentRecord("Student 1")).Entity;
        var parent1 = new Parent("Mariano", "john@gmail.com", "1111");
        var parent2 = new Parent("Marie", "aut_marie@gmail.com", "222");
        studentRecord.AddParent(parent1);
        studentRecord.AddParent(parent2, "Aunt");

        dataContext.SaveChanges();
        var id = studentRecord.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        studentRecord = dataContext.StudentRecords
            .Include(s => s.CaregivingRelationships).ThenInclude(cgr => cgr.Parent)
            .Where(s => s.Id == id).FirstOrDefault();

        parent1 = dataContext.Parents.First(p => p.Name == "Mariano");
        parent2 = dataContext.Parents.First(p => p.Name == "Marie");
        Assert.That(studentRecord, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(studentRecord.CaregivingRelationships.First(r => r.Parent == parent1).Name, Is.EqualTo("Parent"));
            Assert.That(studentRecord.CaregivingRelationships.First(r => r.Parent == parent2).Name, Is.EqualTo("Aunt"));
        }
    }
    #endregion

    #region Notifications

    [Test]
    public void CanPersistANotification()
    {
        var notification = new Notification("Hello world", DateTime.UtcNow);
        notification.AddRecipient("Teacher 1", "teacher-1@school.edu", "111");
        notification.AddRecipient("Teacher 2", "teacher-2@school.edu", "222");

        dataContext.Notifications.Add(notification);

        dataContext.SaveChanges();
        var id = notification.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        notification = dataContext.Notifications.Find(id);

        Assert.That(notification, Is.Not.Null);
        Assert.That(notification.Recipients, Has.Count.EqualTo(2));
    }

    [Test]
    public void PersistSentAtTime()
    {
        var notification = new Notification("Hello world", DateTime.UtcNow);
        notification.AddRecipient("Teacher 1", "teacher-1@school.edu", "111");
        notification.AddRecipient("Teacher 2", "teacher-2@school.edu", "222");

        dataContext.Notifications.Add(notification);

        dataContext.SaveChanges();
        var id = notification.Id;

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        notification = dataContext.Notifications.Find(id);
        Assert.That(notification?.SentAt, Is.Null);

        var timeProvider = new TestTimeProvider(new DateTime(2025, 10, 10));
        notification?.SendIfItIsTime(new TestNotificator(), timeProvider);
        
        dataContext.SaveChanges();

        dataContext.Dispose();
        dataContext = CreateContext(tenantProvider);

        notification = dataContext.Notifications.Find(id);

        Assert.That(notification?.SentAt, Is.EqualTo(timeProvider.UtcNow));
    }

    #endregion

    #region Multitenancy

    [Test]
    public void ShouldNotReturnAConfigurationOwnedByADiferentTenant()
    {
        AssertNotAccesibleFor(new Configuration("MyName", "A value"));
    }

    [Test]
    public void ShouldNotReturnAGradeOwnedByADiferentTenant()
    {
        AssertNotAccesibleFor(new Grade("10th grade"));
    }

    [Test]
    public void ShouldNotReturnANotificationOwnedByADiferentTenant()
    {
        AssertNotAccesibleFor(new Notification("Message", DateTime.UtcNow));
    }

    [Test]
    public void ShouldNotReturnAParentOwnedByADiferentTenant()
    {
        AssertNotAccesibleFor(new Parent("John Doe", "", ""));
    }

    [Test]
    public void ShouldNotReturnAStudentOwnedByADiferentTenant()
    {
        AssertNotAccesibleFor(new StudentRecord("Student 1"));
    }

    [Test]
    public void ShouldNotReturnATeacherOwnedByADiferentTenant()
    {
        AssertNotAccesibleFor(new Teacher("Mr. Teacher", "", ""));
    }

    private void AssertNotAccesibleFor<T>(T entityToAdd) where T : TenantEntity
    {
        const int MAIN_TENANT_ID = 8;
        const int OTHER_TENANT_ID = 9;

        var testTenantProvider = new TestTenantProvider(MAIN_TENANT_ID);
        dataContext = CreateContext(testTenantProvider);

        T entity = dataContext.Set<T>().Add(entityToAdd).Entity;

        dataContext.SaveChanges();

        var propertyInfo = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        var id = propertyInfo?.GetValue(entity);

        Assert.That(dataContext.Set<T>().Find(id), Is.Not.Null);

        dataContext.Dispose();

        testTenantProvider.SetTenantId(OTHER_TENANT_ID);
        dataContext = CreateContext(testTenantProvider);

        Assert.That(dataContext.Set<T>().Find(id), Is.Null);
    }

    #endregion
}
