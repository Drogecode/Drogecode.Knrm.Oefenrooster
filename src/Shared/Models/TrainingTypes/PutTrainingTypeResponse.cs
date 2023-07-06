using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

public class PutTrainingTypeResponse : BaseResponse
{
    public Guid? NewId { get; set; }
}
