using Model;
using Model.Ports.Driven;
using Model.Ports.Driving;
using Persistence;

namespace Application;

public class Notifications : INotifications
{
    private readonly UnitOfWork unitOfWork;
    private readonly IRegistrar registrar;
    private readonly ITimeProvider timeProvider;
    private readonly Secretary secretary;

    public Notifications(UnitOfWork unitOfWork, IRegistrar registrar, INotificator notificator, ITimeProvider timeProvider)
    {
        this.unitOfWork = unitOfWork;
        this.registrar = registrar;
        this.timeProvider = timeProvider;

        secretary = new Secretary(registrar, notificator, timeProvider);
    }

    public void SendGeneral(string message, DateTime scheduleAt = default)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var builder = new GeneralNotificationBuilder(registrar, message, BringToNowPastScheduleDates(scheduleAt));
        secretary.SendNotification(builder.Build());

        unitOfWork.SaveChanges();
    }

    public void SendToGrade(int gradeId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var grade = registrar.Grades.FirstOrDefault(grade => grade.Id == gradeId) ??
            throw new ArgumentException("Invalid grade identifier", nameof(gradeId));

        var builder = new GradeNotificationBuilder(grade, message);
        secretary.SendNotification(builder.Build());

        unitOfWork.SaveChanges();
    }

    public void SendStudent(int studentId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var student = registrar.Students.FirstOrDefault(s => s.Id == studentId) ??
            throw new ArgumentException("Invalid student identifier", nameof(studentId));

        var builder = new StudentNotificationBuilder(student, message);
        secretary.SendNotification(builder.Build());

        unitOfWork.SaveChanges();
    }

    public void SendDisciplinary(int studentId, int teacherId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var student = registrar.Students.FirstOrDefault(s => s.Id == studentId) ??
            throw new ArgumentException("Invalid student identifier", nameof(studentId));

        var teacher = registrar.Teachers.FirstOrDefault(s => s.Id == teacherId) ??
            throw new ArgumentException("Invalid teacher identifier", nameof(teacherId));

        var builder = new DisciplinaryNotificationBuilder(registrar, student, teacher, message);
        secretary.SendNotification(builder.Build());

        unitOfWork.SaveChanges();
    }

    public void SendScheduledNotifications()
    {
        secretary.SendScheduledNotifications();

        unitOfWork.SaveChanges();
    }

    private DateTime BringToNowPastScheduleDates(DateTime scheduleAt)
    {
        return scheduleAt == default ? timeProvider.UtcNow : scheduleAt;
    }
}
