using System.Diagnostics;
using System.Net;

namespace Remote_Shutdown.Core;

internal sealed class RemoteShutdownService
{
    private static ILogger<WindowsBackgroundService> _logger;
    internal static async Task Run(ILogger<WindowsBackgroundService> logger)
    {
        _logger = logger;

        var configuration = new Configuration.ConfigurationManager(Path.Combine(AppContext.BaseDirectory, "config.json")).Load();
        var portNumber = configuration.PortNumber;
        var token = configuration.Token;

        var localHost = await LocalIPv4Address().ConfigureAwait(false);
        var url = $"http://{localHost}:{portNumber}/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();

        WriteLog($"Listening for shutdown requests at {url}", ConsoleColor.Cyan);

        while (true)
        {
            var context = await listener.GetContextAsync().ConfigureAwait(false);
            var requestUrl = context.Request.Url;
            var response = context.Response;

            WriteLog("Request received!", ConsoleColor.Green);
            
            if (requestUrl != null && requestUrl.Query.Contains("shutdown=true")
                                   && requestUrl.Query.Contains($"token={token}"))
            {
                WriteLog("Shutdown command received!", ConsoleColor.Green);

                await SendResponse(response, "Shutting down..").ConfigureAwait(false);

                Process.Start("shutdown", "-s -f -t 0");

                break;
            }

            WriteLog("Invalid request received!", ConsoleColor.Red);

            await SendResponse(response, "Invalid request.").ConfigureAwait(false);
        }
    }

    private static async Task<string> LocalIPv4Address()
    {
        var hostEntry = await Dns.GetHostEntryAsync(Dns.GetHostName()).ConfigureAwait(false);

        return (from ip in hostEntry.AddressList
                where ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                select ip.ToString()).FirstOrDefault();
    }

    private static async Task SendResponse(HttpListenerResponse response, string s)
    {
        var buffer = System.Text.Encoding.UTF8.GetBytes($"<html><body>{s}</body></html>");
        response.ContentLength64 = buffer.Length;

        var output = response.OutputStream;
        await output.WriteAsync(buffer).ConfigureAwait(false);
        output.Close();
    }

    internal static void WriteLog(string log, ConsoleColor consoleColor)
    {
        if (!ServiceChecker.IsRunningAsService())
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} {log}");
            Console.ResetColor();
        }
        else
        {
            _logger.LogInformation(log);
        }
    }
}