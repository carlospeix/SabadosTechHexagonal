


namespace Model;

public class Student
{
    public int Id { get; private set; }

    public string Name { get; private set; }
    public Grade? Grade { get; private set; }

    public IReadOnlyCollection<Parent> Parents => parents.ToList().AsReadOnly();
    private readonly HashSet<Parent> parents = [];

    public IReadOnlyCollection<CaregivingRelationship> CaregivingRelationships => caregivingRelationships.ToList().AsReadOnly();
    private readonly HashSet<CaregivingRelationship> caregivingRelationships = [];

    private Student() {}

    public Student(string name)
    {
        Name = name;
    }

    public void AddParent(Parent parent, string? relationshipName = null)
    {
        caregivingRelationships.Add(new CaregivingRelationship(this, parent, relationshipName));
    }
}
