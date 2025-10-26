using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class CopyToClipboard
{
    // https://chrissainty.com/copy-to-clipboard-in-blazor/
    [Inject, NotNull] IStringLocalizer<CopyToClipboard>? L { get; set; }
    [Inject, NotNull] private IJSRuntime? JSRuntime { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? TextField { get; set; }
    [Parameter] public string? TrustedHtmlText { get; set; }

    private async Task CopyTextToClipboard(string text)
    {
        await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", text, L["Copied to clipboard"].ToString());
    }
}
