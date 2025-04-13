namespace Model;

public abstract class Notification
{
    public string Message { get; init; }

    public abstract IEnumerable<Recipient> GetRecipients();
}