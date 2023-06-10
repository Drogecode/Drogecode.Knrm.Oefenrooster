using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class SharePointListBase
{
    public string? Description { get; set; }
    public string? Title { get; set; }
    public DateTime Start { get; set; }
    public List<SharePointUser> Users { get; set; } = new List<SharePointUser>();
}
