using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class CopyToClipboard
{
    // https://chrissainty.com/copy-to-clipboard-in-blazor/
    [Inject] IStringLocalizer<CopyToClipboard> L { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Parameter] public string Text { get; set; }

    private async Task CopyTextToClipboard()
    {
        await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", Text, L["Copied to clipboard"].ToString());
    }
}
