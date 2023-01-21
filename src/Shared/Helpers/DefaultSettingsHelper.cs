using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Helpers;

public static class DefaultSettingsHelper
{
    public static Guid KnrmHuizenId { get; set; } = new Guid("c0f17c92-57e5-4ec2-83c4-904160078c38");
    public static Guid HRBId { get; set; } = new Guid("fba4d79a-a774-4afa-b432-bc01bbcb68a2");
}
