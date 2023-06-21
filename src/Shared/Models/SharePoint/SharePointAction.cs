using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class SharePointAction : SharePointListBase
{
    public double Number { get; set; }
    public string? ShortDescription { get; set; }
    public string? Prio { get; set; }
}