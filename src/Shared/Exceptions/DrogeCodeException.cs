using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Exceptions;

[Serializable]
public abstract class DrogeCodeException : Exception
{
    public DrogeCodeException()
    {
    }

    public DrogeCodeException(string message)
        : base(message)
    {
    }

    public DrogeCodeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
