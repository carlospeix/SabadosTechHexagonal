using Model;
using Model.Ports.Driven;
using Model.Ports.Driving;

namespace Application;

public class Notifications : INotifications
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;

    public Notifications(IRegistrar registrar, INotificator notificator)
    {
        this.registrar = registrar;
        this.notificator = notificator;
    }

    public void SendGeneral(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var secretary = new Secretary(notificator);
        secretary.SendNotification(new GeneralNotification(registrar, message));
    }

    public void SendToGrade(int gradeId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var grade = registrar.Grades.FirstOrDefault(grade => grade.Id == gradeId) ??
            throw new ArgumentException("Invalid grade identifier", nameof(gradeId));

        var secretary = new Secretary(notificator);
        secretary.SendNotification(new GradeNotification(grade, message));
    }

    public void SendStudent(int studentId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be null or empty", nameof(message));
        }

        var student = registrar.Students.FirstOrDefault(s => s.Id == studentId) ??
            throw new ArgumentException("Invalid student identifier", nameof(studentId));

        var secretary = new Secretary(notificator);
        secretary.SendNotification(new StudentNotification(student, message));
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

        var secretary = new Secretary(notificator);
        secretary.SendNotification(new DisciplinaryNotification(registrar, student, teacher, message));
    }
}
