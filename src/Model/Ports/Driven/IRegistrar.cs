namespace Model.Ports.Driven;

public interface IRegistrar
{
    public Teacher? TeacherById(int teacherId);

    public IQueryable<Grade> AllGrades { get; }
    public Grade? GradeById(int gradeId);

    public IQueryable<Parent> AllParents { get; }

    public Student? StudentById(int studentId);

    public Configuration? ConfigurationByName(string name);

    public IQueryable<Notification> PendingNotificationsBy(DateTime utcNow);
    void AddNotification(Notification notification);
}
