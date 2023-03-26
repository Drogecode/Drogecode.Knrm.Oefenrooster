using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IGraphService
{
    void InitializeGraph(Settings settings);

    Task<string?> GetAccessTokenAsync();

    Task<UserCollectionResponse?> ListUsersAsync();
    Task<DirectoryObjectCollectionResponse?> GetGroupForUser(string userId);
}
