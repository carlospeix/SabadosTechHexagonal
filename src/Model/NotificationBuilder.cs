namespace Model;

public abstract class NotificationBuilder(string message, DateTime scheduleAt)
{
    public string Message { get; init; } = message;
    public DateTime ScheduleAt { get; init; } = scheduleAt;

    protected abstract void AddRecipientsTo(Notification notification);

    public Notification Build()
    {
        var notification = new Notification(Message, ScheduleAt);

        AddRecipientsTo(notification);

        return notification;
    }

    protected void AddGradeRecipientsTo(Grade grade, Notification notification)
    {
        foreach (var subject in grade.Subjects)
        {
            notification.AddRecipient(subject.Teacher.Name, subject.Teacher.Email, subject.Teacher.Phone);
        }
        foreach (var student in grade.Students)
        {
            AddStudentRecipientsTo(student, notification);
        }
    }

    protected void AddStudentRecipientsTo(Student student, Notification notification)
    {
        foreach (var cgr in student.CaregivingRelationships)
        {
            notification.AddRecipient(cgr.Parent.Name, cgr.Parent.Email, cgr.Parent.Phone);
        }
    }
}
