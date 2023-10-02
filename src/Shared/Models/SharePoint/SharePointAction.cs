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
    public string? Type { get; set; }
    public string? Request { get; set; }
    public string? ForTheBenefitOf { get; set; }
    public string? Causes { get; set; }
    public string? Implications { get; set; }
    public DateTime Commencement { get; set; }
}