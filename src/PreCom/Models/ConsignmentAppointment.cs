using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.PreCom.Models;

public class ConsignmentAppointment
{
    public long ServiceFunctionID { get; set; }
    public DateTime Start { get; set; }
    public DateTime Stop { get; set; }
}
