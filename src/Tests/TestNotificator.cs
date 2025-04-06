using Model;
using Model.Ports.Driven;

namespace Tests;

internal class TestNotificator : INotificator
{
    readonly ILogger logger = NullLogger.Instance;

    private int notificationCount;

    public TestNotificator()
    {
    }

    public TestNotificator(ILogger<TestNotificator> logger)
    {
        this.logger = logger;
    }

    public void Send(IEnumerable<Recipient> recipients, string message)
    {
        notificationCount = 0;
        foreach (var recipient in recipients)
        {
            logger.LogInformation(
                "Sending notification: {message} to {name}, {email}, {phone}",
                message, recipient.Name, recipient.Email, recipient.Phone);
            notificationCount++;
        }
    }

    internal int NotificationsSent()
    {
        return notificationCount;
    }
}
