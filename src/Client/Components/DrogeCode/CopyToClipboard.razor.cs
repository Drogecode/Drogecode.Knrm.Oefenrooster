using Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class CopyToClipboard
{
    // https://chrissainty.com/copy-to-clipboard-in-blazor/
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Parameter] public string Text { get; set; }

    private async Task CopyTextToClipboard()
    {
        await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", Text);
    }
}
