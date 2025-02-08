using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User.Components;

public partial class ActivateEmailDialog : IDisposable
{
    [Inject] private IStringLocalizer<ActivateEmailDialog> L { get; set; } = default!;
    [Inject] private IUserLinkedMailsClient UserLinkedMailsClient { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public Guid EmailId { get; set; }

    private readonly CancellationTokenSource _cls = new();
    private string? _activationKey;
    private bool? _validated;

    private async Task Validate()
    {
        _validated = null;
        StateHasChanged();
        if (_activationKey is null || !_activationKey.Length.Equals(DefaultSettingsHelper.LENGTH_MAIL_ACTIVATION))
        {
            _validated = false;
            StateHasChanged();
            return;
        }

        var body = new ValidateUserLinkedActivateKeyRequest()
        {
            UserLinkedMailId = EmailId,
            ActivationKey = _activationKey
        };
        var result = await UserLinkedMailsClient.ValidateUserLinkedActivateKeyAsync(body, _cls.Token);
        _validated = result.Success;
        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        if (result.Success)
        {
            MudDialog.Close();
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}