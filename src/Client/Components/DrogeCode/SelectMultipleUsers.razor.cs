using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class SelectMultipleUsers
{
    [Inject, NotNull] private IStringLocalizer<SelectMultipleUsers>? L { get; set; }
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public EventCallback<IEnumerable<DrogeUser>> SelectionChanged { get; set; }
    [Parameter] public bool MultiSelection { get; set; } = true;
    [Parameter] public bool Clearable { get; set; } = true;
    [Parameter] public bool ReadOnly { get; set; } = false;
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public bool Disabled { get; set; } = false;

    private IEnumerable<DrogeUser?>? _selectedUsers = new List<DrogeUser>();
    [Parameter]
    public IEnumerable<DrogeUser?>? Selection
    {
        get => _selectedUsers;
        set
        {
            if (Equals(_selectedUsers, value)) return;
            _selectedUsers = value;
            SelectionChanged.InvokeAsync(value);
        }
    }
    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(Label))
            Label = L["From who"];
    }

    private static string GetMultiSelectionText(List<DrogeUser> selectedValues)
    {
        var result = string.Join(", ", selectedValues.Select(x => x.Name));
        return result;
    }

    private static string GetSelectedText(DrogeUser selectedValue)
    {
        return selectedValue.Name;
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser?>? selection)
    {
        Selection = selection;
    }
}
