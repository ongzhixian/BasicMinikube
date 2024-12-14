namespace WareLogix.ReverseProxy;

public class AppConfig
{
    public string Mode { get; set; } = "ReverseProxy";

    public string[] ReverseProxyRoutesConfigs { get; set; } = [];

    public bool IsHttpForwarder => Mode.Equals("HttpForwarder", StringComparison.InvariantCultureIgnoreCase);
    
}
