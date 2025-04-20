namespace Model;

public class Recipient
{
    protected int? Id;

    public Notification Notification { get; private set; }
    public string Name { get; }
    public string Email { get; }
    public string Phone { get; }

    private Recipient() { }

    public Recipient(Notification notification, string name, string email, string phone)
    {
        Notification = notification;
        Name = name;
        Email = email;
        Phone = phone;
    }
}
