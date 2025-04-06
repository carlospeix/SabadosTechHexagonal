using Model;
using Model.Ports.Driven;

namespace WebApi;

public class NullNotificator : INotificator
{
    public void Send(IEnumerable<Recipient> recipients, string message)
    {
        foreach (var _ in recipients)
        {
            // noop
        }
    }
}
