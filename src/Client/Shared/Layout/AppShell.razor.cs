using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;

public partial class AppShell
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter, EditorRequired] public  bool IsDarkMode { get; set; }
    [Parameter, EditorRequired] public  DarkLightMode DarkModeToggle { get; set; }
    [Parameter, EditorRequired, NotNull] public  MudTheme? MyCustomTheme  { get; set; }
    public MudThemeProvider MudThemeProvider { get; set; } = new();
}