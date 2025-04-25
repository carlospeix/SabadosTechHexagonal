namespace Model;

public class GradeNotificationBuilder : NotificationBuilder
{
    private readonly Grade grade;

    public GradeNotificationBuilder(Grade grade, string message) : base(message, default)
    {
        this.grade = grade;
    }

    protected override Task AddRecipientsTo(Notification notification)
    {
        AddGradeRecipientsTo(grade, notification);

        return Task.CompletedTask;
    }
}
