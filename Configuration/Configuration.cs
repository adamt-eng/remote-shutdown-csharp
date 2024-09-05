namespace Remote_Shutdown.Configuration;

public class Configuration
{
    public string Token { get; set; } // Any token you choose, just make sure to keepit secret as to prevent unauthorized shutdowns
    public int PortNumber { get; set; } // Recommended Range: 1024-49151
}