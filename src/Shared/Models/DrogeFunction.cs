using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public class DrogeFunction
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public bool TrainingOnly { get; set; }
    public bool Default { get; set; }
}
