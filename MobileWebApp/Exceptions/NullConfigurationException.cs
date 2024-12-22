namespace MobileWebApp.Exceptions;

public class NullConfigurationException : ArgumentNullException
{
    public NullConfigurationException(string? configurationKey) : base(configurationKey)
    {
    }
}
