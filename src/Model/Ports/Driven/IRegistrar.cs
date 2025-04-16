namespace Model.Ports.Driven;

public interface IRegistrar
{
    public IQueryable<Teacher> Teachers { get; }
    public IQueryable<Grade> Grades { get; }
    public IQueryable<Parent> Parents { get; }
    public IQueryable<Student> Students { get; }
    public IQueryable<Configuration> Configurations { get; }
    public IQueryable<Notification> Notifications { get; }
    // TODO: This should be in another place
    void AddNotification(Notification notification);
}
