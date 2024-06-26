﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
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
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private RefreshModel _refreshModel = new();
    private CancellationTokenSource _cls = new();
    private List<DefaultGroup>? _defaultGroups { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _defaultGroups = (await _defaultScheduleRepository.GetAllGroups(_cls.Token))?.OrderBy(x => x.ValidFrom).ToList();
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private string GetGroupTitle(DefaultGroup group)
    {
        var groupName = group.Name;
        if (group.IsDefault && string.IsNullOrEmpty(group.Name))
        {
            groupName = L["Default group name"];
        }
        if (group.IsDefault)
            return groupName;
        return $"{groupName} {group.ValidFrom!.Value.ToLocalTime().ToShortDateString()} {LApp["till"]} {group.ValidUntil!.Value.ToLocalTime().ToShortDateString()}";
    }

    private void OpenGroupDialog(DefaultGroup? group, bool isNew)
    {
        var parameters = new DialogParameters<GroupDialog> {
            { x=>x.DefaultGroup, group},
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
        };
        DialogOptions options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        _dialogProvider.Show<GroupDialog>(L["Add group"], parameters, options);
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
