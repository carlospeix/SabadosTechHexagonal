namespace Model;

/// <summary>
/// Port de ports & adapters
/// </summary>
public interface INotificationSender
{
    public void Send(Recipient recipient, string message);
}
