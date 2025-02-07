namespace Drogecode.Knrm.Oefenrooster.Shared.Exceptions;

[Serializable]
public class DrogeCodeNullException : DrogeCodeException
{
    public DrogeCodeNullException()
    {
    }

    public DrogeCodeNullException(string message)
        : base(message)
    {
    }

    public DrogeCodeNullException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
