using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer
{
    public class GraphTests
    {
        private IGraphService _graphService;
        public GraphTests(IGraphService graphService)
        {
            _graphService = graphService;
        }

        [Fact]
        public async Task InitializeGraphTest()
        {
            _graphService.InitializeGraph();
            var token = await _graphService.GetAccessTokenAsync();
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task GetUserListTest()
        {
            _graphService.InitializeGraph();
            var userPage = await _graphService.ListUsersAsync();
            Assert.NotNull(userPage?.Value);
            Assert.NotEmpty(userPage.Value);
        }

        [Fact]
        public async Task GetGroupForUserTest()
        {
            _graphService.InitializeGraph();
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

    }
}