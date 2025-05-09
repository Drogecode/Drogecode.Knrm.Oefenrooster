using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Drogecode.Knrm.Oefenrooster.SharedForTests.Helpers;
using Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserLinkCustomerControllerTests : BaseTest
{
    public UserLinkCustomerControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task LinkUserToCustomerTest()
    {
        var globalUserId = SeedUserGlobal.Seed(Tester.DataContext);
        var result = await UserLinkCustomerController(Tester.DefaultUserId, globalUserId);
        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetAllUserLinkCustomersTest()
    {
        var globalUserId = SeedUserGlobal.Seed(Tester.DataContext);
        await UserLinkCustomerController(Tester.DefaultUserId, globalUserId);
        var getAllResult = await Tester.UserLinkCustomerController.GetAllUsersWithLinkToCustomer(Tester.DefaultCustomerId);
        Assert.NotNull(getAllResult?.Value?.LinkInfo);
        Assert.True(getAllResult.Value.Success);
        Assert.NotEmpty(getAllResult.Value.LinkInfo);
        getAllResult.Value.LinkInfo.Should().Contain(x => x.DrogeUser != null && x.DrogeUser.Id == Tester.DefaultUserId && x.UserGlobal != null && x.UserGlobal.Id == globalUserId);
    }

    [Fact]
    public async Task GetUserLinkCustomerTest()
    {
        var globalUserId = SeedUserGlobal.Seed(Tester.DataContext);
        await UserLinkCustomerController(DefaultSettingsHelperMock.IdTaco, globalUserId);
        var getAllResult = await Tester.UserLinkCustomerController.GetAllCustomersLinkedToMe();
        Assert.NotNull(getAllResult?.Value?.UserLinkedCustomers);
        Assert.True(getAllResult.Value.Success);
        Assert.NotEmpty(getAllResult.Value.UserLinkedCustomers);
        getAllResult.Value.UserLinkedCustomers.Should().Contain(x => x.CustomerId == Tester.DefaultCustomerId);
    }

    private async Task<LinkUserToCustomerResponse?> UserLinkCustomerController(Guid userId, Guid globalUserId)
    {
        var body = new LinkUserToCustomerRequest()
        {
            CustomerId = Tester.DefaultCustomerId,
            UserId = userId,
            GlobalUserId = globalUserId,
            IsActive = true,
            CreateNew = true,
        };
        var result = await Tester.UserLinkCustomerController.LinkUserToCustomer(body);
        return result.Value;
    }
}