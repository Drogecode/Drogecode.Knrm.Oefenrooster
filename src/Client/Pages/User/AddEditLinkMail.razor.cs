using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public partial class AddEditLinkMail : IDisposable
{
    [Inject] private IStringLocalizer<AddEditLinkMail> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IUserLinkedMailsClient UserLinkedMailsClient { get; set; } = default!;
    [Parameter] public Guid? Id { get; set; }
    [AllowNull] private MudForm _form = null!;
    private readonly CancellationTokenSource _cls = new();
    private string? _email;
    private bool _success;
    private string[] _errors = Array.Empty<string>();

    private async Task Submit()
    {
        await _form.Validate();
        if (!_form.IsValid) return;

        var body = new PutUserLinkedMailRequest()
        {
            SendMail = true,
            UserLinkedMail = new UserLinkedMail()
            {
                Email = _email
            }
        };
        var putResponse = await UserLinkedMailsClient.PutUserLinkedMailAsync(body, _cls.Token);
        if (putResponse.Success)
        {
            Navigation.NavigateTo("/user/profile");
            return;
        }

        switch (putResponse.Error)
        {
            case PutUserLinkedMailError.MailAlreadyExists:
                _errors = new[] { L["Mail already exists"].ToString() };
                break;
            case PutUserLinkedMailError.TooMany:
                _errors = new[] { L["Too many"].ToString() };
                break;
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _form.Dispose();
    }
}