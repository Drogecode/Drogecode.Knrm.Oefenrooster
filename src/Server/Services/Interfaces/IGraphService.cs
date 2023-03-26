using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IGraphService
{
    void InitializeGraph();

    Task<string?> GetAccessTokenAsync();

    Task<UserCollectionResponse?> ListUsersAsync();
    Task<DirectoryObjectCollectionResponse?> GetGroupForUser(string userId);
    Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse users);
}
