using Model;
using Model.Ports.Driven;

namespace Tests;

internal class TestNotificator : INotificator
{
    private int notificationCount;

    public void Send(IEnumerable<Recipient> recipients, string message)
    {
        notificationCount = 0;
        foreach (var _ in recipients)
        {
            notificationCount++;
        }
    }

    internal int NotificationsSent()
    {
        return notificationCount;
    }
}
