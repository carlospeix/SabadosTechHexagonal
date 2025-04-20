namespace Model.Ports.Driven;

public interface INotificator
{
    public Task Send(IEnumerable<Recipient> recipients, string message);
}
