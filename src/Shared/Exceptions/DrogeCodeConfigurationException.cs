namespace Drogecode.Knrm.Oefenrooster.Shared.Exceptions;

[Serializable]
public class DrogeCodeConfigurationException : DrogeCodeException
{
    public DrogeCodeConfigurationException()
    {
    }

    public DrogeCodeConfigurationException(string message)
        : base(message)
    {
    }

    public DrogeCodeConfigurationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}