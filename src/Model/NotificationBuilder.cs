namespace Model;

public abstract class NotificationBuilder
{
    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }

    protected abstract IEnumerable<Recipient> GetRecipients();

    protected NotificationBuilder(string message, DateTime scheduleAt)
    {
        Message = message;
        ScheduleAt = scheduleAt;
    }

    public Notification Build()
    {
        return new Notification(GetRecipients(), Message, ScheduleAt);
    }

    protected IEnumerable<Recipient> GradeRecipients(Grade grade)
    {
        foreach (var subject in grade.Subjects)
        {
            yield return subject.Teacher.GetRecipient();
        }
        foreach (var student in grade.Students)
        {
            foreach (var recipient in StudentRecipients(student))
            {
                yield return recipient;
            }
        }
    }

    protected IEnumerable<Recipient> StudentRecipients(Student student)
    {
        foreach (var parent in student.Parents)
        {
            yield return parent.GetRecipient();
        }
    }
}
