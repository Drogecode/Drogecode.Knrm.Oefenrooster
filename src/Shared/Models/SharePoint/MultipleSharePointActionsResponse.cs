using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class MultipleSharePointActionsResponse : BaseResponse
{
    public List<SharePointAction>? SharePointActions { get; set; }
}
