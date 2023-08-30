using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
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
        DataContext dataContext,
        IDateTimeService dateTimeServiceMock,
        ScheduleController scheduleController,
        FunctionController functionController,
        UserController userController,
        HolidayController holidayController,
        TrainingTypesController trainingTypesController,
        CalendarItemController calendarItemController,
        PreComController preComController,
        VehicleController vehicleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController, vehicleController)
    {
    }

    [Fact]
    public async Task WebHookTest()
    {
        string body = "{\"_data\":{\"actionData\":{\"MsgOutID\":\"129660402\",\"ControlID\":\"f\",\"Timestamp\":\"2023-06-15T11:50:07.203\"},\"remote\":true,\"notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"priority\":\"10\"},\"_remoteNotificationCompleteCallbackCalled\":false,\"_isRemote\":true,\"_notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"_alert\":\"Nee!!! \",\"_sound\":\"pager.caf\",\"_contentAvailable\":1}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(Guid.NewGuid(), asObject, false);

        var result = await PreComController.AllAlerts(CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x=>x.Alert.Equals("Nee!!! "));
    }

    [Fact]
    public async Task WebHookBodyIssueSoundFieldTest()
    {
        string body = "{\"_data\":{\"actionData\":{\"MsgOutID\":\"133556490\",\"ControlID\":\"g\",\"Timestamp\":\"2023-08-11T15:17:43.373\"},\"remote\":true,\"notificationId\":\"BBC7FAFF-AD81-40C8-BE37-C2CE8CD4EE8F\",\"priority\":\"10\"},\"_remoteNotificationCompleteCallbackCalled\":false,\"_isRemote\":true,\"_notificationId\":\"BBC7FAFF-AD81-40C8-BE37-C2CE8CD4EE8F\",\"_alert\":\"Uitruk voorstel:\\r\\nKNRM schipper: 1\\r\\nHUI Mark van den Brink\\r\\nKNRM opstapper: 1\\r\\nHUI Ferry Mol\\r\\nKNRM algemeen: 2\\r\\nHUI Laurens Klijn,\\r\\nHUI Ruben de Ronde\\r\\n\\r\\nNiet ingedeeld:\\r\\nHUI Laurens van Slooten\\r\\n\\r\\nHUI PRIO 2 MARITIEME HULPVERLENING\",\"_sound\":{\"name\":\"pager.caf\",\"volume\":1,\"critical\":0},\"_contentAvailable\":1}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(Guid.NewGuid(), asObject, false);

        var result = await PreComController.AllAlerts(CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x=>x.Alert.Equals("Uitruk voorstel:\r\nKNRM schipper: 1\r\nHUI Mark van den Brink\r\nKNRM opstapper: 1\r\nHUI Ferry Mol\r\nKNRM algemeen: 2\r\nHUI Laurens Klijn,\r\nHUI Ruben de Ronde\r\n\r\nNiet ingedeeld:\r\nHUI Laurens van Slooten\r\n\r\nHUI PRIO 2 MARITIEME HULPVERLENING"));
    }
}
