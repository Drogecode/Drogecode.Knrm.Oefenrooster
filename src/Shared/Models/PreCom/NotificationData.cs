using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class NotificationData
{
    public Data _data { get; set; }
    public bool _remoteNotificationCompleteCallbackCalled { get; set; }
    public bool _isRemote { get; set; }
    public Guid _notificationId { get; set; }
    public string _alert { get; set; }
    public string _sound { get; set; }
    public int _contentAvailable { get; set; }
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
    public string MsgOutID { get; set; }
    public string ControlID { get; set; }
    public string Timestamp { get; set; }
}