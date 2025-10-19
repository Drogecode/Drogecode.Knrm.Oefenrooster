using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Models.Palettes;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public partial class ExternalsLayout
{
    [Inject] private IStringLocalizer<MainLayout> L { get; set; } = default!;
    private readonly DrogeCodeGlobal _global = new();
    private readonly DarkLightMode _darkModeToggle = DarkLightMode.System;
    private AppShell _appShell = new();
    private bool _isDarkMode;

    private readonly MudTheme _myCustomTheme = new()
    {
        PaletteLight = new KnrmPaletteLight(),
        PaletteDark = new KnrmPaletteDark(),
    };
}