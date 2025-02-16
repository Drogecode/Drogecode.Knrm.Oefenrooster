namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests;

public class BaseTest(TestService testService) : IAsyncLifetime
{
    protected TestService Tester { get; private set; } = testService;

    public virtual async Task InitializeAsync()
    {
        await Tester.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}