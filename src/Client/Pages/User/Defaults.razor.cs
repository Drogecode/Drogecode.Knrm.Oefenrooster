using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using static MudBlazor.CategoryTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Defaults : IDisposable
{
    [Inject] private IStringLocalizer<Defaults> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private RefreshModel _refreshModel = new();
    private CancellationTokenSource _cls = new();
    private List<DefaultGroup>? _defaultGroups { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _defaultGroups = (await _defaultScheduleRepository.GetAllGroups(_cls.Token))?.OrderBy(x => x.ValidFrom).ToList();
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private string GetGroupName(DefaultGroup group)
    {
        if (group.IsDefault && string.IsNullOrEmpty(group.Name))
        {
            return L["Default group name"];
        }
        return group.Name;
    }
    private async Task RefreshMeAsync()
    {
        _defaultGroups = (await _defaultScheduleRepository.GetAllGroups(_cls.Token))?.OrderBy(x => x.ValidFrom).ToList();
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
