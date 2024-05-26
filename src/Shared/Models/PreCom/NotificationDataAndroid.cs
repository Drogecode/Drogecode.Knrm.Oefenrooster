using System.Text.Json.Serialization;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class NotificationDataAndroid
{
    public DataAndroid data { get; set; }
    public string from { get; set; }
    public string messageId { get; set; }
    public long sentTime { get; set; }
    public long ttl { get; set; }
}

public class DataAndroid
{
    public string android_channel_id { get; set; }
    [JsonPropertyName("content-available")]
    public string ContentAvailable { get; set; }
    public string message { get; set; }
    public MessageDataAndroid messageData { get;set;}
    public string notId { get; set; }
    public string soundname { get; set; }
    public string vibrationPattern { get; set; }
}

public class MessageDataAndroid
{
    public string MsgOutID { get; set; }
    public string ControlID { get; set; }
    public string Timestamp { get; set; }
}

/*

{
    "data": {
        "android_channel_id": "vibrate",
        "content-available": "1",
        "message": "U bent ingedeeld als KNRM Aank. Opstapper\r\n\r\nPrio 1, Vaartuig motor / stuur problemen, HUI",
        "messageData": {
            "MsgOutID": "149048711",
            "ControlID": "g",
            "Timestamp": "2024-04-05T13:25:19.21"
        },
        "notId": "149048711",
        "soundname": "",
        "vibrationPattern": "[500,500,500,500,500,500]"
    },
    "from": "788942585741",
    "messageId": "0:1712323519628673%af1e7638f9fd7ecd",
    "sentTime": 1712323519591,
    "ttl": 2419200
}

*/