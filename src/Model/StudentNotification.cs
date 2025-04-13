namespace Model;

public class StudentNotification
{
    public string Message { get; init; }

    private Student student;

    public StudentNotification(Student student, string message)
    {
        this.student = student;
        Message = message;
    }

    public IEnumerable<Recipient> GetRecipients()
    {
        foreach (var parent in student.Parents)
        {
            yield return new Recipient(parent.Name, parent.Email, parent.Phone);
        }
    }
}
