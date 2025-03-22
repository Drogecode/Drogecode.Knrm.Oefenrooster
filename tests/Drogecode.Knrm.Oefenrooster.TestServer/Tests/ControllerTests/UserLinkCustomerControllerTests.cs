using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserLinkCustomerControllerTests : BaseTest
{
    public UserLinkCustomerControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task<Guid> LinkUserToCustomerTest()
    {
        var body = new LinkUserToCustomerRequest()
        {
            CustomerId = Tester.SecondaryCustomerId,
            UserId = Tester.DefaultUserId,
            IsActive = true,
            CreateNew = true,
        };
        var result = await Tester.UserLinkCustomerController.LinkUserToCustomer(body);
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
        return newUser.Value.User.Id;
    }

    [Fact]
    public async Task GetAllUserLinkCustomersTest()
    {
        var linkedUserId = await LinkUserToCustomerTest();
        var getAllResult = await Tester.UserLinkCustomerController.GetAllUsersWithLinkToCustomer(Tester.SecondaryCustomerId);
        Assert.NotNull(getAllResult?.Value?.LinkInfo);
        Assert.True(getAllResult.Value.Success);
        Assert.NotEmpty(getAllResult.Value.LinkInfo);
        getAllResult.Value.LinkInfo.Should().Contain(x => x.DrogeUserOther != null && x.DrogeUserOther.Id == linkedUserId && x.DrogeUserCurrent != null && x.DrogeUserCurrent.Id == Tester.DefaultUserId);
    }
}