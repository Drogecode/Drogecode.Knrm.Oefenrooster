using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class NotificationData
{
    public Data Data { get; set; }
    public bool remoteNotificationCompleteCallbackCalled { get; set; }
    public bool IsRemote { get; set; }
    public Guid NotificationId { get; set; }
    public string Alert { get; set; }
    public string Sound { get; set; }
    public int ContentAvailable { get; set; }
}
public class Data
{
    public ActionData ActionData { get; set; }
    public bool Remote { get; set; }
    public Guid NotificationId { get; set; }
    public string Priority { get; set; }
}

public class ActionData
{
    public string MsgOutID { get; set; }
    public string ControlID { get; set; }
    public DateTime Timestamp { get; set; }
}