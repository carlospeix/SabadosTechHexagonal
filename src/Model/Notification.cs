namespace Model;

public abstract class Notification
{
    public string Message { get; init; }

    public abstract IEnumerable<Recipient> GetRecipients();

    protected IEnumerable<Recipient> GradeRecipients(Grade grade)
    {
        foreach (var subject in grade.Subjects)
        {
            yield return new Recipient(subject.Teacher.Name, subject.Teacher.Email, subject.Teacher.Phone);
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
            yield return new Recipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
