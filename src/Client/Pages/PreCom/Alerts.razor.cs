using Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;

public sealed partial class Alerts : IDisposable
{
    [Inject] private IStringLocalizer<Alerts> L { get; set; } = default!;
    [Inject] private PreComRepository PreComRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<PreComAlert>? _alerts;
    private DrogeUser? _user;
    private bool _showHowTo;
    protected override async Task OnInitializedAsync()
    {
        _alerts = await PreComRepository.GetAll(_cls.Token);
        _user = await UserRepository.GetCurrentUserAsync(_cls.Token);
    }

    public static string JsonPrettify(string? json)
    {
        if ( string.IsNullOrEmpty(json))
        {
            return string.Empty;
        }
        using var jDoc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
