﻿using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class CopyToClipboard
{
    // https://chrissainty.com/copy-to-clipboard-in-blazor/
    [Inject] IStringLocalizer<CopyToClipboard> L { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Parameter] public string? Label { get; set; }
    [Parameter] public string? TextField { get; set; }
    [Parameter] public string? TrustedHtmlText { get; set; }

    private async Task CopyTextToClipboard(string text)
    {
        await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", text, L["Copied to clipboard"].ToString());
    }
}
