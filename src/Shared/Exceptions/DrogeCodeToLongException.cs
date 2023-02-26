namespace Drogecode.Knrm.Oefenrooster.Shared.Exceptions;

[Serializable]
public class DrogeCodeToLongException : Exception
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
