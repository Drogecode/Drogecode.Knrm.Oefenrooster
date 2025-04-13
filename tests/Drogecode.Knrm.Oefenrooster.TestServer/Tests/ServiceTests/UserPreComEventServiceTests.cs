using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ServiceTests;

public class UserPreComEventServiceTests : BaseTest
{
    private readonly IUserPreComEventService _preComEventService;

    public UserPreComEventServiceTests(
        IUserPreComEventService preComEventService,
        TestService testService) :
        base(testService)
    {
        _preComEventService = preComEventService;
    }

    [Fact]
    public async Task GetEventsForUserTest()
    {
        
    }
    
}