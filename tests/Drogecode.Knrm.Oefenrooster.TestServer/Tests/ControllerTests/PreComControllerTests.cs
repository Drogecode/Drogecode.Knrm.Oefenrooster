using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class PreComControllerTests : BaseTest
{
    public PreComControllerTests(
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController) :
        base(scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController)
    {
    }

    [Fact]
    public async Task WebHookTest()
    {
        string body = "{\"_data\":{\"actionData\":{\"MsgOutID\":\"129660402\",\"ControlID\":\"f\",\"Timestamp\":\"2023-06-15T11:50:07.203\"},\"remote\":true,\"notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"priority\":\"10\"},\"_remoteNotificationCompleteCallbackCalled\":false,\"_isRemote\":true,\"_notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"_alert\":\"Nee!!! \",\"_sound\":\"pager.caf\",\"_contentAvailable\":1}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(asObject, false);

        var result = await PreComController.AllAlerts(CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x=>x.Alert.Equals("Nee!!! "));
    }
}
