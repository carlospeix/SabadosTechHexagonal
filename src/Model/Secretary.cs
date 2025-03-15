namespace Model;

public class Secretary
{
    private readonly IRegistrar registrar;
    private INotificationSender notificationSender;

    public Secretary(IRegistrar registrar, INotificationSender notificationSender)
    {
        this.registrar = registrar;
        this.notificationSender = notificationSender;
    }

    public bool SendNotification(string message)
    {
        foreach (var recipient in Recipients())
        {
            notificationSender.Send(recipient, message);
        }

        return true;
    }

    private IEnumerable<Recipient> Recipients()
    {
        foreach (var grade in registrar.Grades)
        {
            foreach (var subject in grade.Subjects)
            {
                yield return new Recipient(subject.Teacher.Name, subject.Teacher.Email, subject.Teacher.Phone);
            }
        }
    }
}
