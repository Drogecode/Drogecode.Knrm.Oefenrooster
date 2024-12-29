﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Models.Palettes;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public partial class ExternalsLayout
{
    [Inject] private IStringLocalizer<MainLayout> L { get; set; } = default!;
    private DrogeCodeGlobal _global { get; set; } = new();
    private MudThemeProvider _mudThemeProvider = new();
    private bool _isDarkMode;

    private MudTheme _myCustomTheme = new()
    {
        PaletteLight = new KnrmPaletteLight(),
        PaletteDark = new KnrmPaletteDark(),
    };

    protected override async Task OnInitializedAsync()
    {
    }
}