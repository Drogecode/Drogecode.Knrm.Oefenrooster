using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components.Dialogs;

public partial class ShareActionDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<ShareActionDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    [Inject, NotNull] private IReportActionSharedClient? ReportActionSharedClient { get; set; }
    [CascadingParameter, NotNull] IMudDialogInstance? MudDialog { get; set; }
    [Parameter] public List<DrogeUser>? SelectedUsersAction { get; set; }
    [Parameter] public List<string>? SelectedActionTypes { get; set; }
    [Parameter] public  string? Search {get; set;}

    private readonly ReportActionSharedConfiguration _sharedConfiguration = new ReportActionSharedConfiguration
    {
        StartDate = DateTime.UtcNow.AddYears(-1),
        EndDate = DateTime.UtcNow,
        ValidUntil = DateTime.UtcNow.AddMonths(1)
    };

    private readonly CancellationTokenSource _cls = new();
    private string? _password = null;
    private Guid? _newId = null;
    private bool _saved = false;

    private async Task Submit()
    {
        if (_saved) return;
        var users = new List<Guid>();
        if (SelectedUsersAction is not null)
        {
            foreach (var user in SelectedUsersAction)
            {
                users.Add(user.Id);
            }
        }
        _sharedConfiguration.SelectedUsers = users;
        _sharedConfiguration.Types = SelectedActionTypes;
        _sharedConfiguration.Search = Search?.Split(',').ToList();
        var putResponse = await ReportActionSharedClient.PutReportActionSharedAsync(_sharedConfiguration);
        if (putResponse is not null)
        {
            _password = putResponse.Password;
            _newId = putResponse.NewId;
            _saved = putResponse.Success;
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}