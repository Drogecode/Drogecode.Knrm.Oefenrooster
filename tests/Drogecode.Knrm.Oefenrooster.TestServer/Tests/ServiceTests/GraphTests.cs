using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ServiceTests
{
    public class GraphTests
    {
        private IGraphService _graphService;
        public GraphTests(IGraphService graphService)
        {
            _graphService = graphService;
        }

        [Fact(Skip = "Do not talk to real SharePoint")]
        public async Task InitializeGraphTest()
        {
            var settings = Settings.LoadSettingsLikeThis();
            _graphService.InitializeGraph(settings);
            var token = await _graphService.GetAccessTokenAsync();
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact(Skip = "Do not talk to real SharePoint")]
        public async Task GetUserListTest()
        {
            var settings = Settings.LoadSettingsLikeThis();
            _graphService.InitializeGraph(settings);
            var userPage = await _graphService.ListUsersAsync();
            Assert.NotNull(userPage?.Value);
            Assert.NotEmpty(userPage.Value);
        }

        [Fact(Skip = "Do not talk to real SharePoint")]
        public async Task GetGroupForUserTest()
        {
            var settings = Settings.LoadSettingsLikeThis();
            _graphService.InitializeGraph(settings);
            var userPage = await _graphService.ListUsersAsync();
            Assert.NotNull(userPage?.Value);
            Assert.NotEmpty(userPage.Value);
            foreach (var user in userPage.Value)
            {
                Assert.NotNull(user.Id);
                var userResult = _graphService.GetGroupForUser(user.Id);
                Assert.NotNull(userResult);
            }
        }

        [Fact(Skip = "Do not talk to real SharePoint")]
        public async Task GetLists()
        {
            var settings = Settings.LoadSettingsLikeThis();
            _graphService.InitializeGraph(settings);
            await _graphService.GetLists();
        }

    }
}