namespace Model;

public class StudentNotification : Notification
{
    private Student student;

    public StudentNotification(Student student, string message)
    {
        this.student = student;
        Message = message;
    }

    public override IEnumerable<Recipient> GetRecipients()
    {
        foreach (var parent in student.Parents)
        {
            yield return new Recipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
