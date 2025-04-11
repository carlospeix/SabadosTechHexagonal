using System.Xml.Linq;

namespace Model;

public class CaregivingRelationship
{
    protected int? Id;

    public Parent Parent { get; private set; }
    public Student Student { get; private set; }
    public string Name { get; private set; }

    private CaregivingRelationship() {}

    public CaregivingRelationship(Student student, Parent parent, string? name)
    {
        Student = student;
        Parent = parent;
        SetName(name);
    }

    public void SetName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Name = nameof(Parent);
        }
        else
        {
            Name = name;
        }
    }
}
