using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Helpers;

public static class DefaultSettingsHelper
{
    public const string CURRENT_VERSION = "v0.0.10";
    public static Guid KnrmHuizenId { get; } = new Guid("d9754755-b054-4a9c-a77f-da42a4009365");
    //public static Guid HRBId { get; } = new Guid("fba4d79a-a774-4afa-b432-bc01bbcb68a2");
}
