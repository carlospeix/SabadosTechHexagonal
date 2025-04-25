using Model.Ports.Driven;

namespace Model;

public class GeneralNotificationBuilder : NotificationBuilder
{
    private readonly IRegistrar registrar;

    public GeneralNotificationBuilder(IRegistrar registrar, string message, DateTime scheduleAt) : base(message, scheduleAt)
    {
        this.registrar = registrar;
    }

    protected override async Task AddRecipientsTo(Notification notification)
    {
        await foreach (var grade in registrar.AllGrades)
        {
            AddGradeRecipientsTo(grade, notification);
        }
        await foreach (var parent in registrar.AllParents)
        {
            notification.AddRecipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
