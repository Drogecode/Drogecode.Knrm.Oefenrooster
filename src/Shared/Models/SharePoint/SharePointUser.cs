using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class SharePointUser
{
    public string? SharePointID { get; set; }
    public Guid DrogeCodeId { get; set; }
    public string? Name { get; set; }
    public SharePointRole Role { get; set; }
}

public enum SharePointRole
{
    None = 0,
    Schipper = 1,
    Opstapper = 2,
}
