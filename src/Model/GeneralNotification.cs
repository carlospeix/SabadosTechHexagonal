using Model.Ports.Driven;

namespace Model;

public class GeneralNotification : Notification
{
    private readonly IRegistrar registrar;

    public GeneralNotification(IRegistrar registrar, string message)
    {
        this.registrar = registrar;
        Message = message;
    }

    public override  IEnumerable<Recipient> GetRecipients()
    {
        foreach (var grade in registrar.Grades)
        {
            foreach (var recipient in GradeRecipients(grade))
            {
                yield return recipient;
            }
        }
        foreach (var parent in registrar.Parents)
        {
            yield return new Recipient(parent.Name, parent.Email, parent.Phone);
        }
    }

    private IEnumerable<Recipient> GradeRecipients(Grade grade)
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
