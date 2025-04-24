namespace Model;

public class Grade : TenantEntity
{
    public int Id { get; private set; }

    public string StudentName { get; private set; }

    public IReadOnlyCollection<Subject> Subjects => subjects.ToList().AsReadOnly();
    private readonly HashSet<Subject> subjects = [];

    public IReadOnlyCollection<StudentRecord> StudentRecords => studentRecords.ToList().AsReadOnly();
    private readonly HashSet<StudentRecord> studentRecords = [];

    private Grade() {}

    public Grade(string studentName)
    {
        StudentName = studentName;
    }

    public Subject AddSubject(Teacher teacher, string subjectName)
    {
        var subject = new Subject(this, subjectName, teacher);

        subjects.Add(subject);

        return subject;
    }

    public void AddStudent(StudentRecord studentRecord)
    {
        if (studentRecords.Contains(studentRecord))
            return;
        
        studentRecords.Add(studentRecord);
    }
}
