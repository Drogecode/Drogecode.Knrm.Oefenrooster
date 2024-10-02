using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserLinkedMailsService : IUserLinkedMailsService
{
    public async Task<PutUserLinkedMailResponse> PutUserLinkedMail(UserLinkedMail userLinkedMail, Guid customerId, Guid userId, CancellationToken clt)
    {
        throw new NotImplementedException();
    }

    public async Task<PatchUserLinkedMailResponse> PatchUserLinkedMail(UserLinkedMail userLinkedMail, Guid customerId, Guid userId, CancellationToken clt)
    {
        throw new NotImplementedException();
    }

    public async Task<AllUserLinkedMailResponse> AllUserLinkedMail(int take, int skip, Guid userId, Guid customerId, CancellationToken clt)
    {
        throw new NotImplementedException();
    }
}