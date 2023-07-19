using Microsoft.AspNetCore.Components;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks
{
    public class MockNavigationManager
        : NavigationManager
    {
        public MockNavigationManager() : base() =>
            Initialize("http://localhost:2112/", "http://localhost:2112/test");

        protected override void NavigateToCore(string uri, bool forceLoad) =>
            WasNavigateInvoked = true;

        public bool WasNavigateInvoked { get; private set; }
    }
}
