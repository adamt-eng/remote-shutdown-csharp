using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Remote_Shutdown.Core;

namespace Remote_Shutdown;

internal class Program
{
    internal static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddWindowsService(options => { options.ServiceName = "Remote Shutdown"; });

        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

        builder.Logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information);

        builder.Services.AddSingleton<RemoteShutdownService>();
        builder.Services.AddHostedService<WindowsBackgroundService>();

        builder.Build().Run();
    }
}