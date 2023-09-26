﻿using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class TrainingsTab : IDisposable
{
    [Inject] private IStringLocalizer<TrainingsTab> L { get; set; } = default!;
    [Inject] private SharePointRepository _sharePointRepository { get; set; } = default!;
    [Parameter][EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    private List<SharePointTraining>? _sharePointTrainings;
    private CancellationTokenSource _cls = new();
    private IEnumerable<DrogeUser> _selectedUsersTraining = new List<DrogeUser>();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _sharePointTrainings = await _sharePointRepository.GetLastTrainingsForCurrentUser(10, 0, _cls.Token);
            if (Users is not null)
            {
                var thisUser = Users!.FirstOrDefault(x => x.Id == User.Id);
                if (thisUser is not null)
                {
                    ((List<DrogeUser>)_selectedUsersTraining).Add(thisUser);
                }
            }
            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        _selectedUsersTraining = selection;
        _sharePointTrainings = await _sharePointRepository.GetLastTrainings(selection, 10, 0, _cls.Token);
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}