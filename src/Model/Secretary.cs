using Model.Ports.Driven;
using System.Diagnostics;

namespace Model;

public class Secretary
{
    private readonly IRegistrar registrar;
    private readonly INotificator notificator;

    public Secretary(IRegistrar registrar, INotificator notificator)
    {
        this.registrar = registrar;
        this.notificator = notificator;
    }

    public void SendGeneralNotification(string message)
    {
        notificator.Send(AllRecipients(), message);
    }

    public void SendGradeNotification(Grade grade, string message)
    {
        notificator.Send(GradeRecipients(grade), message);
    }

    public void SendStudentNotification(Student student, string message)
    {
        notificator.Send(StudentRecipients(student), message);
    }

    private IEnumerable<Recipient> AllRecipients()
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
