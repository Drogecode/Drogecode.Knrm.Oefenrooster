using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class CustomerControllerTests : BaseTest
{
    public CustomerControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task LinkUserToCustomerTest()
    {
        var body = new LinkUserToCustomerRequest()
        {
            CustomerId = Tester.SecondaryCustomerId,
            UserId = Tester.DefaultUserId,
            IsActive = true,
            CreateNew = true,
        };
        var result = await Tester.CustomerController.LinkUserToCustomer(body);
        Assert.NotNull(result?.Value);
        Assert.True(result.Value.Success);
        Assert.NotNull(result.Value.NewUserId);
        
        var defaultUser = await Tester.UserController.GetById(Tester.DefaultUserId);
        Assert.NotNull(defaultUser?.Value?.User?.Name);
        var newUser = await Tester.UserController.GetById(result.Value.NewUserId.Value);
        Assert.NotNull(newUser?.Value);
        Assert.True(newUser.Value.Success);
        newUser.Value.User.Should().NotBeNull();
        newUser.Value.User.Name.Should().Be(defaultUser.Value.User.Name);
    }
}