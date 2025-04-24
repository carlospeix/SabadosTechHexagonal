namespace Model;

public class StudentRecord : TenantEntity
{
    public int Id { get; private set; }

    public string StudentName { get; private set; }

    public Grade? CurrentGrade { get; private set; }

    public IReadOnlyCollection<Parent> Parents => parents.ToList().AsReadOnly();
    private readonly HashSet<Parent> parents = [];

    public IReadOnlyCollection<CaregivingRelationship> CaregivingRelationships => caregivingRelationships.ToList().AsReadOnly();
    private readonly HashSet<CaregivingRelationship> caregivingRelationships = [];

    private StudentRecord() {}

    public StudentRecord(string name)
    {
        StudentName = name;
    }

    public void AddParent(Parent parent, string? relationshipName = null)
    {
        caregivingRelationships.Add(new CaregivingRelationship(this, parent, relationshipName));
    }
}
