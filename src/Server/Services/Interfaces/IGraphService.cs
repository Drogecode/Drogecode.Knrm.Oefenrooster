using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IGraphService
{
    void InitializeGraph(Settings? settings = null);

    Task<string?> GetAccessTokenAsync();

    Task<UserCollectionResponse?> ListUsersAsync();
    Task<DirectoryObjectCollectionResponse?> GetGroupForUser(string userId);
    Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse users);
    Task GetLists();
    Task<object> GetListTrainingUser(string userName, Guid userId, int count, Guid customerId, CancellationToken clt);
}
