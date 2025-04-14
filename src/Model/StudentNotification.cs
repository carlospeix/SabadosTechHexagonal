namespace Model;

public class StudentNotification : Notification
{
    private readonly Student student;

    public StudentNotification(Student student, string message)
    {
        this.student = student;
        Message = message;
    }

    public override IEnumerable<Recipient> GetRecipients()
    {
        foreach (var recipient in StudentRecipients(student))
        {
            yield return recipient;
        }
    }
}
