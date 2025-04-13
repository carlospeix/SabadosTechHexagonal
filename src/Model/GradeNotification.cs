namespace Model;

public class GradeNotification : Notification
{
    private readonly Grade grade;

    public GradeNotification(Grade grade, string message)
    {
        this.grade = grade;
        Message = message;
    }

    public override IEnumerable<Recipient> GetRecipients()
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

    private IEnumerable<Recipient> StudentRecipients(Student student)
    {
        foreach (var parent in student.Parents)
        {
            yield return new Recipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
