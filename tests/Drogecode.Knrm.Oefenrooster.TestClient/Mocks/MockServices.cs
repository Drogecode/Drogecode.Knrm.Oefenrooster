using RichardSzalay.MockHttp;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks;

public static class MockServices
{
    public static MockHttpMessageHandler AddMockServices(this TestServiceProvider services)
    {
        var mockHttpHandler = new MockHttpMessageHandler();
        var httpClient = mockHttpHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost");
        services.AddSingleton<HttpClient>(httpClient);
        return mockHttpHandler;
    }
}