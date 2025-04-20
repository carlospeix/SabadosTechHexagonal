using Model;
using Model.Ports.Driven;

namespace WebApi;

public class NullNotificator : INotificator
{
    public Task Send(IEnumerable<Recipient> recipients, string message)
    {
        foreach (var _ in recipients)
        {
            // noop
        }

        return Task.CompletedTask;
    }
}
