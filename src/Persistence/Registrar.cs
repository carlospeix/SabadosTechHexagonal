using Model;
using Model.Ports.Driven;
using System.Linq.Expressions;

namespace Persistence;

public class Registrar(ApplicationContext applicationContext) : IRegistrar
{
    private readonly ApplicationContext applicationContext = applicationContext;

    public Teacher? TeacherById(int teacherId) =>
        applicationContext.Teachers.FirstOrDefault(s => s.Id == teacherId);

    public IQueryable<Grade> AllGrades =>
        applicationContext.Grades;
    public Grade? GradeById(int gradeId) =>
        applicationContext.Grades.FirstOrDefault(g => g.Id == gradeId);

    public IQueryable<Parent> AllParents =>
        applicationContext.Parents;

    public Student? StudentById(int studentId) =>
        applicationContext.Students.FirstOrDefault(s => s.Id == studentId);

    public Configuration? ConfigurationByName(string name)
        => applicationContext.Configurations.FirstOrDefault(c => c.Name == name);

    public IAsyncEnumerable<Notification> FilteredNotifications(Expression<Func<Notification, bool>> filter) =>
        applicationContext.Notifications.Where(filter).AsAsyncEnumerable();

    public void AddNotification(Notification notification)
    {
        applicationContext.Notifications.Add(notification);
    }
}
