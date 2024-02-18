using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class SelectMultipleUsers
{
    [Inject] private IStringLocalizer<SelectMultipleUsers> L { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    [Parameter] public EventCallback<IEnumerable<DrogeUser>> SelectionChanged { get; set; }
    [Parameter] public bool MultiSelection { get; set; } = true;
    [Parameter] public bool Clearable { get; set; } = true;
    [Parameter] public bool ReadOnly { get; set; } = false;
    [Parameter] public string Label { get; set; } = string.Empty;

    private IEnumerable<DrogeUser> _selectedUsers = new List<DrogeUser>();
    [Parameter]
    public IEnumerable<DrogeUser> Selection
    {
        get
        {
            return _selectedUsers;
        }
        set
        {
            if (_selectedUsers == value) return;
            _selectedUsers = value;
            SelectionChanged.InvokeAsync(value);
        }
    }
    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(Label))
            Label = L["From who"];
    }

    private string GetMultiSelectionText(List<DrogeUser> selectedValues)
    {
        var result = string.Join(", ", selectedValues.Select(x => x.Name));
        return result;
    }

    private string GetSelectedText(DrogeUser selectedValue)
    {
        return selectedValue.Name;
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        Selection = selection;
    }
}
