using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class LicenseControllerTests : BaseTest
{
    public LicenseControllerTests(TestService testService) : base(testService)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        SeedLicense.Seed(Tester.DataContext, Tester.DefaultCustomerId, Licenses.LicenseSharePointReports);
    }

    [Fact]
    public async Task GetAllLicensesForCustomerTest()
    {
        var result = await Tester.LicenseController.GetLicenseForCurrentCustomer();
        Assert.NotNull(result?.Value?.Licenses);
        Assert.True(result.Value.Success);
        Assert.NotEmpty(result.Value.Licenses);
        result.Value.Licenses.Should().Contain(x => x.License == Licenses.LicenseSharePointReports);
        result.Value.Licenses.Should().NotContain(x => x.License == Licenses.LicensePreCom);
    }

    [Fact]
    public async Task GetAllLicensesOneWithFromTillInTimeTest()
    {
        SeedLicense.Seed(Tester.DataContext, Tester.DefaultCustomerId, Licenses.LicensePreCom, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5));
        var result = await Tester.LicenseController.GetLicenseForCurrentCustomer();
        Assert.NotNull(result?.Value?.Licenses);
        Assert.True(result.Value.Success);
        Assert.NotEmpty(result.Value.Licenses);
        result.Value.Licenses.Should().Contain(x => x.License == Licenses.LicenseSharePointReports);
        result.Value.Licenses.Should().Contain(x => x.License == Licenses.LicensePreCom);
    }
    
    [Fact]
    public async Task GetAllLicensesButNotValidTest()
    {
        SeedLicense.Seed(Tester.DataContext, Tester.DefaultCustomerId, Licenses.LicensePreCom, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(10));
        var result = await Tester.LicenseController.GetLicenseForCurrentCustomer();
        Assert.NotNull(result?.Value?.Licenses);
        Assert.True(result.Value.Success);
        Assert.NotEmpty(result.Value.Licenses);
        result.Value.Licenses.Should().Contain(x => x.License == Licenses.LicenseSharePointReports);
        result.Value.Licenses.Should().NotContain(x => x.License == Licenses.LicensePreCom);
    }
    
    [Fact]
    public async Task GetAllLicensesButOneExpiredTest()
    {
        SeedLicense.Seed(Tester.DataContext, Tester.DefaultCustomerId, Licenses.LicensePreCom, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-1));
        var result = await Tester.LicenseController.GetLicenseForCurrentCustomer();
        Assert.NotNull(result?.Value?.Licenses);
        Assert.True(result.Value.Success);
        Assert.NotEmpty(result.Value.Licenses);
        result.Value.Licenses.Should().Contain(x => x.License == Licenses.LicenseSharePointReports);
        result.Value.Licenses.Should().NotContain(x => x.License == Licenses.LicensePreCom);
    }
    
    [Fact]
    public async Task GetAllLicensesButFromTillReversedTest()
    {
        SeedLicense.Seed(Tester.DataContext, Tester.DefaultCustomerId, Licenses.LicensePreCom, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(-5));
        var result = await Tester.LicenseController.GetLicenseForCurrentCustomer();
        Assert.NotNull(result?.Value?.Licenses);
        Assert.True(result.Value.Success);
        Assert.NotEmpty(result.Value.Licenses);
        result.Value.Licenses.Should().Contain(x => x.License == Licenses.LicenseSharePointReports);
        result.Value.Licenses.Should().NotContain(x => x.License == Licenses.LicensePreCom);
    }
}