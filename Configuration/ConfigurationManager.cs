using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Remote_Shutdown.Core;

namespace Remote_Shutdown.Configuration;

public class ConfigurationManager(string configFilePath)
{
    private readonly JsonSerializerSettings _jsonSettings = new() { ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented };

    public Configuration Load()
    {
        if (!File.Exists(configFilePath))
        {
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(new Configuration(), _jsonSettings));
            
            RemoteShutdownService.WriteLog($"Please fill in the required settings in {configFilePath}.", ConsoleColor.Red);
            RemoteShutdownService.WriteLog("Press enter to exit.", ConsoleColor.Yellow);
            Console.Read();
            
            Environment.Exit(1);
        }
        else
        {
            var configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configFilePath), _jsonSettings);
            if (configuration.Token == null || configuration.PortNumber == 0)
            {
                RemoteShutdownService.WriteLog($"One or more properties in {configFilePath} are not set properly.", ConsoleColor.Red);
                RemoteShutdownService.WriteLog("Press enter to exit.", ConsoleColor.Yellow);
                Console.Read();
                
                Environment.Exit(1);
            }
            else
            {
                return configuration;
            }
        }

        throw new InvalidOperationException("Unreachable code.");
    }
}
