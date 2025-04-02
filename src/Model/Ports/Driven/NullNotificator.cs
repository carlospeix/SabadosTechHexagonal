
namespace Model.Ports.Driven;

public class NullNotificator : INotificator
{
    public void Send(IEnumerable<Recipient> recipients, string message)
    {
    }
}
