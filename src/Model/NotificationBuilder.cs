namespace Model;

public abstract class NotificationBuilder
{
    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }

    protected abstract void AddRecipientsTo(Notification notification);

    protected NotificationBuilder(string message, DateTime scheduleAt)
    {
        Message = message;
        ScheduleAt = scheduleAt;
    }

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
        foreach (var parent in student.Parents)
        {
            notification.AddRecipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
