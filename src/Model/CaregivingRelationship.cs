namespace Model;

public class CaregivingRelationship
{
    protected int? Id;

    public Parent Parent { get; private set; }
    public StudentRecord StudentRecord { get; private set; }
    public string Name { get; private set; }

    private CaregivingRelationship() {}

    public CaregivingRelationship(StudentRecord studentRecord, Parent parent, string? name)
    {
        StudentRecord = studentRecord;
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
