using Drogecode.Knrm.Oefenrooster.Client.Models;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class LandingLayout
{
    private DrogeCodeGlobal _global { get; set; } = new();
    private MudThemeProvider _mudThemeProvider = new();
    private bool _isDarkMode;
}
