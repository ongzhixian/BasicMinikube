namespace MobileWebApp.Exceptions;

public class ConfigurationNullException : ArgumentNullException
{
    public ConfigurationNullException(string? configurationKey) : base(configurationKey)
    {
    }
}

