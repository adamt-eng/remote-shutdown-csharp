namespace Remote_Shutdown.Core;

internal sealed class WindowsBackgroundService(ILogger<WindowsBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RemoteShutdownService.Run(logger).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Message}", ex.Message);
            Environment.Exit(1);
        }
    }
}