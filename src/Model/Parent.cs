namespace Model;

public class Parent
{
    public int Id { get; private set; }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

    private Parent() {}

    public Parent(string name, string email, string phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }
}
