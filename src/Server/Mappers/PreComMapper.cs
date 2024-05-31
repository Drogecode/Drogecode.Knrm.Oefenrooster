using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class PreComMapper
{
    public static DbPreComForward ToDb(this PreComForward forward, Guid customerId, Guid userId)
    {
        return new DbPreComForward
        {
            Id = forward.Id,
            ForwardUrl = forward.ForwardUrl ?? throw new NullReferenceException("ForwardUrl is empty"),
            CustomerId = customerId,
            UserId = userId,
        };
    }
    public static PreComForward ToPreComForward(this DbPreComForward dbForward)
    {
        return new PreComForward
        {
            Id = dbForward.Id,
            ForwardUrl = dbForward.ForwardUrl,
        };
    }
}