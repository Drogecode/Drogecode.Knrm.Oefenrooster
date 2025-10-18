using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode.Dialogs;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public partial class ConfirmSwitch : ComponentBase
{
    [Inject, NotNull] private IStringLocalizer<ConfirmSwitch>? L { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    [Parameter] public bool Value { get; set; }
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Default;
    [Parameter] public bool Disabled { get; set; }

    private async Task OnValueChanged(bool value)
    {
        var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.Value, value },
            { x => x.Body, L["New value {0}", L[value ? "enabled" : "disabled"]] },
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true
        };
        var dialog = await DialogProvider.ShowAsync<ConfirmDialog>(L["Confirm"], parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: bool response })
        {
            Value = response;
            await ValueChanged.InvokeAsync(response);
            StateHasChanged();
        }
    }
}