using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.PreCom.Models;

public class MsgSend
{
    public long SendBy { get; set; } = -1;
    public bool Priority { get; set; } = false;
    public bool Response { get; set; } = false;
    public long CalculateGroupID { get; set; } = -1;
    public DateTime ValidFrom { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<MsgReceivers> Receivers { get; set; }
}

public class MsgReceivers
{
    public int Type { get; set; }
    public long ID { get; set; }
    public string Label { get; set; }
}
