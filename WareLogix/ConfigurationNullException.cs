namespace WareLogix;

public class ConfigurationNullException : ArgumentNullException
{
    public ConfigurationNullException()
    {
    }

    public ConfigurationNullException(string? paramName) : base(paramName)
    {
    }
}

