
namespace Model;

public class Parent : TenantEntity
{
    public int Id { get; private set; }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

    public IReadOnlyCollection<Student> Students => students.ToList().AsReadOnly();
    private readonly HashSet<Student> students = [];

    private Parent() {}

    public Parent(string name, string email, string phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }
}
