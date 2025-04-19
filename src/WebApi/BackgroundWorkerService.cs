using Model.Ports.Driving;

namespace WebApi;

public class BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<BackgroundWorkerService> logger = logger;
    private readonly IServiceProvider serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("Background worker running at {time}.", DateTimeOffset.Now);

            // Resolve INotifications within a scope
            using (var scope = serviceProvider.CreateScope())
            {
                var notifications = scope.ServiceProvider.GetRequiredService<INotifications>();
                await notifications.SendPendingNotifications(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMilliseconds(2000), stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Background worker service starting...");

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Background worker service stopping...");

        return base.StopAsync(cancellationToken);
    }
}