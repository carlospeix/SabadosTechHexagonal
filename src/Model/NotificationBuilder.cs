namespace Model;

public abstract class NotificationBuilder
{
    public string Message { get; init; }
    public DateTime ScheduleAt { get; init; }

    public abstract Notification Build();

    protected NotificationBuilder(string message, DateTime scheduleAt = default)
    {
        Message = message;
        ScheduleAt = scheduleAt;
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
