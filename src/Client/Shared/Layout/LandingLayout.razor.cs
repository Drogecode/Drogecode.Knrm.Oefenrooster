using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Models.Palettes;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public sealed partial class LandingLayout
{
    private DrogeCodeGlobal _global { get; set; } = new();
    private bool _isDarkMode;
    private DarkLightMode _darkModeToggle;
    private AppShell _appShell = new();

    private MudTheme _myCustomTheme = new()
    {
        PaletteLight = new KnrmPaletteLight(),
        PaletteDark = new KnrmPaletteDark(),
    };
}
