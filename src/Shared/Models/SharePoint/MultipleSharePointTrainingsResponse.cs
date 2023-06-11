using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class MultipleSharePointTrainingsResponse : BaseResponse
{
    public List<SharePointTraining>? SharePointTrainings { get; set; }
}
