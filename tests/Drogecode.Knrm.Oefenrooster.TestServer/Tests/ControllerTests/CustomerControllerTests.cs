using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class CustomerControllerTests : BaseTest
{
    public CustomerControllerTests(TestService testService) : base(testService)
    {
    }

    [Fact]
    public async Task GetAllCustomerTest()
    {
        var result = await Tester.CustomerController.GetAllCustomers();
        Assert.NotNull(result?.Value?.Customers);
        Assert.True(result.Value.Success);
        result.Value.Customers.Should().NotBeEmpty();
        result.Value.Customers.Should().HaveCountGreaterOrEqualTo(2);
        result.Value.Customers.Should().Contain(x => x.Id == Tester.DefaultCustomerId);
        result.Value.Customers.Should().Contain(x => x.Id == Tester.SecondaryCustomerId);
    }

    [Fact]
    public async Task GetCustomerByIdTest()
    {
        var result = await Tester.CustomerController.GetCustomerById(new GetCustomerRequest()
        {
            CustomerId = Tester.DefaultCustomerId,
        });
        Assert.NotNull(result?.Value?.Customer);
        Assert.True(result.Value.Success);
        result.Value.Customer.Name.Should().Be("xUnit customer");
    }

    [Fact]
    public async Task PutNewCustomerTest()
    {
        const string customerName = "xUnit";
        var body = new Customer
        {
            Name = customerName,
        };
        var result = await Tester.CustomerController.PutNewCustomer(body);
        Assert.NotNull(result?.Value?.NewId);
        Assert.True(result.Value.Success);
        var resultGet = await Tester.CustomerController.GetCustomerById(new GetCustomerRequest()
        {
            CustomerId = result.Value.NewId.Value,
        });
        Assert.NotNull(resultGet?.Value?.Customer);
        Assert.True(resultGet.Value.Success);
        resultGet.Value.Customer.Name.Should().Be(customerName);
    }

    [Fact]
    public async Task PatchCustomerTest()
    {
        const string newName = "Patched customer";
        var old = (await Tester.CustomerController.GetCustomerById(new GetCustomerRequest()
        {
            CustomerId = Tester.DefaultCustomerId,
        }))!.Value!.Customer;
        old!.Name = newName;
        var patchedResult = await Tester.CustomerController.PatchCustomer(old);
        Assert.NotNull(patchedResult?.Value);
        Assert.True(patchedResult.Value.Success);
        var newVersion = (await Tester.CustomerController.GetCustomerById(new GetCustomerRequest()
        {
            CustomerId = Tester.DefaultCustomerId,
        }))!.Value!.Customer;
        Assert.NotNull(newVersion);
        newVersion!.Name.Should().Be(newName);
    }
}