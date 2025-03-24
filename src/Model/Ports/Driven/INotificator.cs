namespace Model.Ports.Driven;

public interface INotificator
{
    public void Send(IEnumerable<Recipient> recipients, string message);
}
