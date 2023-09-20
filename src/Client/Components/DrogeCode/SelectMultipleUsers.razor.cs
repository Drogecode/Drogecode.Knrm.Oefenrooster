using Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class SelectMultipleUsers
{
    [Inject] private IStringLocalizer<SelectMultipleUsers> L { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    [Parameter] public EventCallback<IEnumerable<DrogeUser>> SelectionChanged { get; set; }

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

    private string GetMultiSelectionText(List<DrogeUser> selectedValues)
    {
        var result = string.Join(", ", selectedValues.Select(x => x.Name));
        return result;
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        Selection = selection;
    }
}
