using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

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
        DayItemController dayItemController,
        MonthItemController monthItemController,
        PreComController preComController,
        VehicleController vehicleController,
        DefaultScheduleController defaultScheduleController,
        ReportActionController reportActionController,
        ReportTrainingController reportTrainingController,
        UserRoleController userRoleController,
        UserLinkedMailsController userLinkedMailsController) :
        base(dataContext, dateTimeServiceMock, scheduleController, userController, functionController, holidayController, trainingTypesController, dayItemController, monthItemController,
            preComController, vehicleController, defaultScheduleController, reportActionController, reportTrainingController, userRoleController, userLinkedMailsController)
    {
    }

    [Fact]
    public async Task WebHookIOSTest()
    {
        string body = "{\"_data\":{\"actionData\":{\"MsgOutID\":\"129660402\",\"ControlID\":\"f\",\"Timestamp\":\"2023-06-15T11:50:07.203\"},\"remote\":true,\"notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"priority\":\"10\"},\"_remoteNotificationCompleteCallbackCalled\":false,\"_isRemote\":true,\"_notificationId\":\"29A12538-536D-4546-B3C0-188C32521BA8\",\"_alert\":\"Nee!!! \",\"_sound\":\"pager.caf\",\"_contentAvailable\":1}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelperMock.IdTaco, asObject, false);

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
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelperMock.IdTaco, asObject, false);

        var result = await PreComController.AllAlerts(50, 0, CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x => x.Alert.Equals("Uitruk voorstel:\r\nKNRM schipper: 1\r\nHUI Mark van den Brink\r\nKNRM opstapper: 1\r\nHUI Ferry Mol\r\nKNRM algemeen: 2\r\nHUI Laurens Klijn,\r\nHUI Ruben de Ronde\r\n\r\nNiet ingedeeld:\r\nHUI Laurens van Slooten\r\n\r\nHUI PRIO 2 MARITIEME HULPVERLENING"));
        result.Value.PreComAlerts.Should().Contain(x => x.SendTime.Equals(new DateTime(2023, 08, 11, 15, 17, 43, 373)));
    }

    [Fact]
    public async Task WebHookAndroidTest()
    {
        string body = "{\"data\":{\"android_channel_id\":\"vibrate\",\"content-available\":\"1\",\"message\":\"U bent ingedeeld als KNRM Aank. Opstapper\\r\\n\\r\\nPrio 1, Vaartuig motor / stuur problemen, HUI\",\"messageData\":{\"MsgOutID\":\"149048711\",\"ControlID\":\"g\",\"Timestamp\":\"2024-04-05T13:25:19.21\"},\"notId\":\"149048711\",\"soundname\":\"\",\"vibrationPattern\":\"[500,500,500,500,500,500]\"},\"from\":\"788942585741\",\"messageId\":\"0:1712323519628673%af1e7638f9fd7ecd\",\"sentTime\":1712323519591,\"ttl\":2419200}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelperMock.IdTaco, asObject, false);

        var result = await PreComController.AllAlerts(50, 0, CancellationToken.None);
        Assert.NotNull(result?.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x => x.Alert.Equals("U bent ingedeeld als KNRM Aank. Opstapper\r\n\r\nPrio 1, Vaartuig motor / stuur problemen, HUI"));
        result.Value.PreComAlerts.Should().Contain(x => x.SendTime.Equals(new DateTime(2024, 04, 05, 13, 25, 19, 591)));
    }

    [Fact]
    // The webhook test message from when you press WEBHOOKTEST in the app original
    public async Task WebHookBodyIsWebhookTest1Test()
    {
        string body = "{\"android_channel_id\":\"chirp\",\"content- available\":\"1\",\"message\":\"PreCom test bericht voor Webhook\",\"messageData\":{\"MsgOutID\":\"135615552\",\"ControlID\":\"f\",\"Timestamp\":\"1695417214488\",\"notId\":\"135615552\",\"soundname\":\"chirp\",\"vibrationPattern\":\"[150,545]\",\"from\":\"788942585741\",\"messageId\":\"0:1694527951397184%af1e7638f9fd7ecd\",\"sentTime\":1694527951377,\"ttl\":2419200}}";
        var asObject = JsonSerializer.Deserialize<object>(body);
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelperMock.IdTaco, asObject, false);

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
        await PreComController.WebHook(DefaultCustomerId, DefaultSettingsHelperMock.IdTaco, asObject, false);

        var result = await PreComController.AllAlerts(50, 0, CancellationToken.None);
        Assert.NotNull(result.Value?.PreComAlerts);
        Assert.True(result.Value.Success);
        result.Value.PreComAlerts.Should().NotBeNull();
        result.Value.PreComAlerts.Should().NotBeEmpty();
        result.Value.PreComAlerts.Should().Contain(x => x.Alert.Equals("PreCom test bericht voor Webhook"));
        result.Value.PreComAlerts.Should().Contain(x => x.SendTime.Equals(new DateTime(2024, 03, 01, 19, 46, 08, 215)));
    }

    [Fact]
    public async Task PutPreComForwardTest()
    {
        var body = new PreComForward
        {
            ForwardUrl = "https://PutPreComForward.Test"
        };

        var result = await PreComController.PutForward(body);
        Assert.NotNull(result.Value?.NewId);
        Assert.True(result.Value.Success);
        result.Value.NewId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task AllForwardsTest()
    {
        var body = new PreComForward
        {
            ForwardUrl = "https://AllForwardsTest.Test"
        };

        var result = await PreComController.PutForward(body);
        Assert.NotNull(result.Value?.NewId);
        result.Value.NewId.Should().NotBe(Guid.Empty);

        var allResult = await PreComController.AllForwards(30, 0);
        Assert.NotNull(allResult.Value?.PreComForwards);
        allResult.Value.PreComForwards.Should().NotBeEmpty();
        allResult.Value.PreComForwards.Should().Contain(x => x.Id == result.Value.NewId);
    }

    [Fact]
    public async Task PatchForwardTest()
    {
        const string PATCHED = "https://AllForwardsTest.patched";
        var body = new PreComForward
        {
            ForwardUrl = "https://AllForwardsTest.Test"
        };

        var result = await PreComController.PutForward(body);
        Assert.NotNull(result.Value?.NewId);
        result.Value.NewId.Should().NotBe(Guid.Empty);
        body.Id = result.Value.NewId.Value;
        body.ForwardUrl = PATCHED;

        var patchResult = await PreComController.PatchForward(body);
        Assert.NotNull(patchResult.Value?.Success);
        Assert.True(patchResult.Value.Success);

        var allResult = await PreComController.AllForwards(30, 0);
        Assert.NotNull(allResult.Value?.PreComForwards);
        allResult.Value.PreComForwards.Should().NotBeEmpty();
        allResult.Value.PreComForwards.Should().Contain(x => x.Id == result.Value.NewId && x.ForwardUrl == PATCHED);
    }
}
