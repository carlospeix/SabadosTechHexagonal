namespace Model;

public class Recipient
{
    public string Name { get; }
    public string Email { get; }
    public string Phone { get; }

    public Recipient(string name, string email, string phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }
}