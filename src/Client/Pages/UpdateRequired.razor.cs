using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;

public sealed partial class UpdateRequired
{
    [Inject, NotNull] private IStringLocalizer<UpdateRequired>? L { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }

    public async Task ReloadPage()
    {
        Navigation.NavigateTo(Navigation.BaseUri, true);
    }
}