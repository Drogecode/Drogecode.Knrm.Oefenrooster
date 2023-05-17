using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.PreCom.Models;

public class MsgInLog
{
    public int MsgInID { get; set; }
    public DateTime Timestamp { get; set; }
    public string Text { get; set; }
    public Group Group { get; set; }
}
