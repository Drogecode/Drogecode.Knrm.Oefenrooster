namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class NotificationDataBase
{
    public Data _data { get; set; }
    public bool _remoteNotificationCompleteCallbackCalled { get; set; }
    public bool _isRemote { get; set; }
    public Guid _notificationId { get; set; }
    public string _alert { get; set; }
    public string _category { get; set; }
    public int _contentAvailable { get; set; }
}

public class NotificationDataSoundString : NotificationDataBase
{
    public string _soundOld { get; set; }
}
public class NotificationDataSoundObject : NotificationDataBase
{
    public Sound _sound { get; set; }
}
public class NotificationDataTestWebhookObject : NotificationDataBase
{
    public string android_channel_id { get; set; }
    public string message { get; set; }
    public MessageData messageData { get; set; }
}

public class MessageData
{
    public string MsgOutID { get; set; }
    public string ControlID { get; set; }
    public string Timestamp { get; set; }
    public string notId { get; set; }
    public string soundname { get; set; }
    public string vibrationPattern { get; set; }
    public string from { get; set; }
    public string messageId { get; set; }
    public long sentTime { get; set; }
    public int ttl { get; set; }
}

public class Data
{
    public ActionData actionData { get; set; }
    public bool remote { get; set; }
    public Guid notificationId { get; set; }
    public string priority { get; set; }
}

public class ActionData
{
    public string MsgInID { get; set; }
    public string MsgOutID { get; set; }
    public string ControlID { get; set; }
    public DateTime Timestamp { get; set; }
}

public class Sound
{
    public string name { get; set; }
    public int volume { get; set; }
    public int critical { get; set; }
}