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
            throw new InvalidDataException("Message cannot be null or empty");
        }

        var secretary = new Secretary(registrar, notificator);
        secretary.SendGeneralNotification(message);
    }

    public void SendToGrade(int gradeId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new InvalidDataException("Message cannot be null or empty");
        }

        var grade = registrar.Grades.FirstOrDefault(grade => grade.Id == gradeId);
        if (grade is null)
        {
            throw new InvalidDataException("Invalid grade identifier");
        }

        var secretary = new Secretary(registrar, notificator);
        secretary.SendGradeNotification(new GradeNotification(grade, message));
    }

    public void SendStudent(int studentId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new InvalidDataException("Message cannot be null or empty");
        }

        var student = registrar.Students.FirstOrDefault(s => s.Id == studentId);
        if (student is null)
        {
            throw new InvalidDataException("Invalid student identifier");
        }

        var secretary = new Secretary(registrar, notificator);
        secretary.SendStudentNotification(new StudentNotification(student, message));
    }
}
