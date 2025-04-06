using Model.Ports.Driven;
using Model.Ports.Driving;

namespace Model.UseCases;

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
            throw new InvalidParameterException("Message cannot be null or empty");
        }

        var secretary = new Secretary(registrar, notificator);
        secretary.SendGeneralNotification(message);
    }

    public void SendToGrade(int gradeId, string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new InvalidParameterException("Message cannot be null or empty");
        }

        var grade = registrar.Grades.FirstOrDefault(grade => grade.Id == gradeId);
        if (grade is null)
        {
            throw new InvalidParameterException("Invalid grade identifier");
        }

        var secretary = new Secretary(registrar, notificator);
        secretary.SendGradeNotification(grade, message);
    }
}
