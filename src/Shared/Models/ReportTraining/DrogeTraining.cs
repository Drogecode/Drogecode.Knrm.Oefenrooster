using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class DrogeTraining : SharePointListBase
{
    public string? Type { get; set; }
    public string? TypeTraining { get; set; }
}
