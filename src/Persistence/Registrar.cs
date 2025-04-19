using Model;
using Model.Ports.Driven;

namespace Persistence;

public class Registrar : IRegistrar
{
    private readonly ApplicationContext applicationContext;

    public Registrar(ApplicationContext applicationContext)
    {
        this.applicationContext = applicationContext;
    }

    public IQueryable<Teacher> Teachers => applicationContext.Teachers;
    public IQueryable<Grade> Grades => applicationContext.Grades;
    public IQueryable<Parent> Parents => applicationContext.Parents;
    public IQueryable<Student> Students => applicationContext.Students;
    public IQueryable<Configuration> Configurations => applicationContext.Configurations;
    public IQueryable<Notification> Notifications => applicationContext.Notifications;

    public void AddNotification(Notification notification)
    {
        applicationContext.Notifications.Add(notification);
    }

    public void SaveChanges()
    {
        applicationContext.SaveChanges();
    }
}
