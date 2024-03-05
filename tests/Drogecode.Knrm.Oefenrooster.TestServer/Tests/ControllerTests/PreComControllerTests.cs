using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System.Text.Json;

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
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, calendarItemController, preComController,
            vehicleController, defaultScheduleController)
    {
    }

    [Fact]
    public async Task WebHookTest()
    {
        string body = "{\"_data\":{\"actionData\":{\"MsgOutID\":\"129660402\",\"ControlID\":\"f\",\"Timestamp\":\"2023-06-15T11:50:07.203\"},\"remote\":true,\"notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"priority\":\"10\"},\"_remoteNotificationCompleteCallbackCalled\":false,\"_isRemote\":true,\"_notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"_alert\":\"Nee!!! \",\"_sound\":\"pager.caf\",\"_contentAvailable\":1}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelper.IdTaco, asObject, false);

        var result = await PreComController.AllAlerts(50, 0, CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x => x.Alert.Equals("Nee!!! "));
        result.Value.PreComAlerts.Should().Contain(x => x.SendTime.Equals(new DateTime(2023, 06, 15, 11, 50, 07, 203)));
    }

    [Fact]
    public async Task WebHookBodyIssueSoundFieldTest()
    {
        string body = "{\"_data\":{\"actionData\":{\"MsgOutID\":\"133556490\",\"ControlID\":\"g\",\"Timestamp\":\"2023-08-11T15:17:43.373\"},\"remote\":true,\"notificationId\":\"BBC7FAFF-AD81-40C8-BE37-C2CE8CD4EE8F\",\"priority\":\"10\"},\"_remoteNotificationCompleteCallbackCalled\":false,\"_isRemote\":true,\"_notificationId\":\"BBC7FAFF-AD81-40C8-BE37-C2CE8CD4EE8F\",\"_alert\":\"Uitruk voorstel:\\r\\nKNRM schipper: 1\\r\\nHUI Mark van den Brink\\r\\nKNRM opstapper: 1\\r\\nHUI Ferry Mol\\r\\nKNRM algemeen: 2\\r\\nHUI Laurens Klijn,\\r\\nHUI Ruben de Ronde\\r\\n\\r\\nNiet ingedeeld:\\r\\nHUI Laurens van Slooten\\r\\n\\r\\nHUI PRIO 2 MARITIEME HULPVERLENING\",\"_sound\":{\"name\":\"pager.caf\",\"volume\":1,\"critical\":0},\"_contentAvailable\":1}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelper.IdTaco, asObject, false);

        var result = await PreComController.AllAlerts(50, 0, CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x => x.Alert.Equals("Uitruk voorstel:\r\nKNRM schipper: 1\r\nHUI Mark van den Brink\r\nKNRM opstapper: 1\r\nHUI Ferry Mol\r\nKNRM algemeen: 2\r\nHUI Laurens Klijn,\r\nHUI Ruben de Ronde\r\n\r\nNiet ingedeeld:\r\nHUI Laurens van Slooten\r\n\r\nHUI PRIO 2 MARITIEME HULPVERLENING"));
        result.Value.PreComAlerts.Should().Contain(x => x.SendTime.Equals(new DateTime(2023, 08, 11, 15, 17, 43, 373)));
    }

    [Fact]
    // The webhook test message from when you press WEBHOOKTEST in the app original
    public async Task WebHookBodyIsWebhookTest1Test()
    {
        string body = "{\"android_channel_id\":\"chirp\",\"content- available\":\"1\",\"message\":\"PreCom test bericht voor Webhook\",\"messageData\":{\"MsgOutID\":\"135615552\",\"ControlID\":\"f\",\"Timestamp\":\"1695417214488\",\"notId\":\"135615552\",\"soundname\":\"chirp\",\"vibrationPattern\":\"[150,545]\",\"from\":\"788942585741\",\"messageId\":\"0:1694527951397184%af1e7638f9fd7ecd\",\"sentTime\":1694527951377,\"ttl\":2419200}}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelper.IdTaco, asObject, false);

        var result = await PreComController.AllAlerts(50, 0, CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x => x.Alert.Equals("PreCom test bericht voor Webhook"));
        result.Value.PreComAlerts.Should().Contain(x => x.SendTime.Equals(new DateTime(2023, 09, 12, 14, 12, 31, 377)));
    }

    [Fact]
    // The webhook test message from when you press WEBHOOKTEST in the app updated march 2024
    public async Task WebHookBodyIsWebhookTest2Test()
    {
        string body = "{\"android_channel_id\":\"chirp\",\"content- available\":\"1\",\"message\":\"PreCom test bericht voor Webhook\",\"messageData\":{\"MsgOutID\":\"135615552\",\"ControlID\":\"f\",\"Timestamp\":\"2024-03-01T20:46:08.2\",\"notId\":\"135615552\",\"soundname\":\"chirp\",\"vibrationPattern\":\"[150,545]\",\"from\":\"788942585741\",\"messageId\":\"0:1694527951397184%af1e7638f9fd7ecd\",\"sentTime\":\"1709322368215\",\"ttl\":2419200}}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelper.IdTaco, asObject, false);

        var result = await PreComController.AllAlerts(50, 0, CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x => x.Alert.Equals("PreCom test bericht voor Webhook"));
        result.Value.PreComAlerts.Should().Contain(x => x.SendTime.Equals(new DateTime(2024, 03, 01, 19, 46, 08, 215)));
    }
}
