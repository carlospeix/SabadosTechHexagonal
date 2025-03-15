
namespace Model;

public class Grade
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public IReadOnlyCollection<Subject> Subjects => subjects.ToList().AsReadOnly();
    private readonly HashSet<Subject> subjects = [];

    private Grade() {}

    public Grade(string name)
    {
        Name = name;
    }

    public Subject AddSubject(Teacher teacher, string subjectName)
    {
        var subject = new Subject(this, subjectName, teacher);

        subjects.Add(subject);

        return subject;
    }
}
