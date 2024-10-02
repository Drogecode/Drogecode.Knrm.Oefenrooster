using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserLinkedMailsService
{
    Task<PutUserLinkedMailResponse> PutUserLinkedMail(UserLinkedMail userLinkedMail, Guid customerId, Guid userId, CancellationToken clt);
    Task<PatchUserLinkedMailResponse> PatchUserLinkedMail(UserLinkedMail userLinkedMail, Guid customerId, Guid userId, CancellationToken clt);
    Task<AllUserLinkedMailResponse> AllUserLinkedMail(int take, int skip, Guid userId, Guid customerId, CancellationToken clt);
}