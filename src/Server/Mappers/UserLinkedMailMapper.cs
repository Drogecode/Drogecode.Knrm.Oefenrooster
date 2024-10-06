using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class UserLinkedMailMapper
{
    public static DbUserLinkedMails ToDb(this UserLinkedMail linkedMail)
    {
        return new DbUserLinkedMails
        {
            Id = linkedMail.Id,
            Email = linkedMail.Email,
            ActivateRequestedOn = linkedMail.ActivateRequestedOn,
            ActivationFailedAttempts = linkedMail.ActivationFailedAttempts,
            IsActive = linkedMail.IsActive,
            IsEnabled = linkedMail.IsEnabled,
        };
    }
    public static UserLinkedMail ToDrogecode(this DbUserLinkedMails linkedMail)
    {
        return new UserLinkedMail
        {
            Id = linkedMail.Id,
            Email = linkedMail.Email,
            ActivateRequestedOn = linkedMail.ActivateRequestedOn,
            ActivationFailedAttempts = linkedMail.ActivationFailedAttempts,
            IsActive = linkedMail.IsActive,
            IsEnabled = linkedMail.IsEnabled,
        };
    }
}