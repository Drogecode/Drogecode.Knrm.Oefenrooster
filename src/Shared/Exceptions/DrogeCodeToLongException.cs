namespace Drogecode.Knrm.Oefenrooster.Shared.Exceptions;

[Serializable]
public class DrogeCodeToLongException : DrogeCodeException
{
    public DrogeCodeToLongException()
    {
    }

    public DrogeCodeToLongException(string message)
        : base(message)
    {
    }

    public DrogeCodeToLongException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
