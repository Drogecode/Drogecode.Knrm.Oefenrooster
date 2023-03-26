using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class GraphService : IGraphService
{
    public void InitializeGraph(Settings settings)
    {
        GraphHelper.InitializeGraphForAppOnlyAuth(settings);
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var appOnlyToken = await GraphHelper.GetAppOnlyTokenAsync();
            Console.WriteLine($"App-only token: {appOnlyToken}");
            return appOnlyToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting app-only access token: {ex.Message}");
            return null;
        }
    }

    public async Task<UserCollectionResponse?> ListUsersAsync()
    {
        try
        {
            var userPage = await GraphHelper.GetUsersAsync();

            if (userPage?.Value == null)
            {
                Console.WriteLine("No results returned.");
                return null;
            }

            // Output each users's details
            foreach (var user in userPage.Value)
            {
                Console.WriteLine($"User: {user.DisplayName ?? "NO NAME"}");
                Console.WriteLine($"  ID: {user.Id}");
                Console.WriteLine($"  Email: {user.Mail ?? "NO EMAIL"}");
            }

            // If NextPageRequest is not null, there are more users
            // available on the server
            // Access the next page like:
            // var nextPageRequest = new UsersRequestBuilder(userPage.OdataNextLink, _appClient.RequestAdapter);
            // var nextPage = await nextPageRequest.GetAsync();
            var moreAvailable = !string.IsNullOrEmpty(userPage.OdataNextLink);

            Console.WriteLine($"\nMore users available? {moreAvailable}");
            return userPage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
            return null;
        }
    }

    public async Task MakeGraphCallAsync()
    {
        // TODO
    }
}
