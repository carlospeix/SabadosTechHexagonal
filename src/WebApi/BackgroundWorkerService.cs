using System.Threading;

namespace WebApi;

public class BackgroundWorkerService : BackgroundService
{
    private readonly ILogger<BackgroundWorkerService> logger;

    public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Background worker running at {time}.", DateTimeOffset.Now);

            await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
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