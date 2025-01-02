using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class UserLinkedMailsControllerTests : BaseTest
{
    public UserLinkedMailsControllerTests(
        DataContext dataContext,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        DayItemController dayItemController,
        MonthItemController monthItemController,
        PreComController preComController,
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController,
        ReportActionController reportActionController,
        ReportTrainingController reportTrainingController,
        UserRoleController userRoleController,
        UserLinkedMailsController userLinkedMailsController,
        ReportActionSharedController reportActionSharedController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController, reportActionSharedController)
    {
    }

    [Fact]
    public async Task PutUserLinkedMailTest()
    {
        await PutUserLinkedMail();
    }

    [Fact]
    public async Task GetAllUserLinkedMailTest()
    {
        var newId = await PutUserLinkedMail();
        var allResult = await UserLinkedMailsController.AllUserLinkedMail(10, 0);
        Assert.NotNull(allResult?.Value?.UserLinkedMails);
        Assert.True(allResult.Value.Success);
        allResult.Value.UserLinkedMails.Should().NotBeEmpty();
        allResult.Value.UserLinkedMails.Should().Contain(x => x.Id == newId);
    }

    [Fact]
    public async Task ValidateUserLinkedMailWithValidKeyTest()
    {
        var newId = await PutUserLinkedMail();
        var db = await DataContext.UserLinkedMails.FirstOrDefaultAsync(x => x.Id == newId);
        var validateResponse = await UserLinkedMailsController.ValidateUserLinkedActivateKey(new ValidateUserLinkedActivateKeyRequest() { UserLinkedMailId = newId, ActivationKey = db!.ActivateKey!});
        Assert.NotNull(validateResponse.Value?.Success);
        Assert.True(validateResponse.Value.Success);
        var allResult = await UserLinkedMailsController.AllUserLinkedMail(10, 0);
        Assert.NotNull(allResult?.Value?.UserLinkedMails);
        allResult.Value.UserLinkedMails.Should().Contain(x => x.Id == newId && x.IsActive);
    }

    [Fact]
    public async Task ValidateUserLinkedMailWithWrongKeyTest()
    {
        var newId = await PutUserLinkedMail();
        var validateResponse = await UserLinkedMailsController.ValidateUserLinkedActivateKey(new ValidateUserLinkedActivateKeyRequest() { UserLinkedMailId = newId, ActivationKey = "wrongkey"});
        Assert.NotNull(validateResponse.Value?.Success);
        Assert.False(validateResponse.Value.Success);
        var allResult = await UserLinkedMailsController.AllUserLinkedMail(10, 0);
        Assert.NotNull(allResult?.Value?.UserLinkedMails);
        allResult.Value.UserLinkedMails.Should().Contain(x => x.Id == newId && !x.IsActive);
    }

    [Fact]
    public async Task ChangeEnabledTest()
    {
        var newId = await PutUserLinkedMail();
        var db = await DataContext.UserLinkedMails.FirstOrDefaultAsync(x => x.Id == newId);
        await UserLinkedMailsController.ValidateUserLinkedActivateKey(new ValidateUserLinkedActivateKeyRequest() { UserLinkedMailId = newId, ActivationKey = db!.ActivateKey!});
        var enabledResponse =  await UserLinkedMailsController.IsEnabledChanged(new IsEnabledChangedRequest(){UserLinkedMailId = newId, IsEnabled = false});
        Assert.NotNull(enabledResponse.Value?.Success);
        Assert.True(enabledResponse.Value.Success);
    }

    [Fact]
    public async Task ChangeEnabledWhenNotValidatedTest()
    {
        var newId = await PutUserLinkedMail();
        var enabledResponse =  await UserLinkedMailsController.IsEnabledChanged(new IsEnabledChangedRequest(){UserLinkedMailId = newId, IsEnabled = false});
        Assert.NotNull(enabledResponse.Value?.Success);
        Assert.False(enabledResponse.Value.Success);
    }

    private async Task<Guid> PutUserLinkedMail()
    {
        var body = new PutUserLinkedMailRequest
        {
            UserLinkedMail = new UserLinkedMail
            {
                Email = "PutUserLinkedMailTest@xunit.com",
            },
            SendMail = false, // Do not send mail for unit tests
        };
        var result = await UserLinkedMailsController.PutUserLinkedMail(body);
        Assert.NotNull(result?.Value?.Success);
        Assert.Null(result.Value.ActivateKey);
        Assert.True(result.Value.Success);
        Assert.NotNull(result.Value.NewId);
        result.Value.NewId.Should().NotBeEmpty();
        return result.Value.NewId.Value;
    }
}