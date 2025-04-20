using Model;
using Model.Ports.Driven;

namespace Tests;

internal class TestNotificator : INotificator
{
    readonly ILogger logger = NullLogger.Instance;

    private int notificationCount;

    public TestNotificator()
    {
        Reset();
    }

    public TestNotificator(ILogger<TestNotificator> logger)
    {
        this.logger = logger;
    }

    public Task Send(IEnumerable<Recipient> recipients, string message)
    {
        Reset();

        foreach (var recipient in recipients)
        {
            logger.LogInformation(
                "Sending notification: {message} to {name}, {email}, {phone}",
                message, recipient.Name, recipient.Email, recipient.Phone);
            notificationCount++;
        }

        return Task.CompletedTask;
    }

    internal int NotificationsSent()
    {
        return notificationCount;
    }

    internal void Reset()
    {
        notificationCount = 0;
    }
}
