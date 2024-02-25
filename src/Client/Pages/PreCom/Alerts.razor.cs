using Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;

public sealed partial class Alerts : IDisposable
{
    [Inject] private IStringLocalizer<Alerts> L { get; set; } = default!;
    [Inject] private PreComRepository _preComRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<PreComAlert>? _alerts;
    protected override async Task OnInitializedAsync()
    {
        _alerts = await _preComRepository.GetAll(_cls.Token);
    }

    public void Dispose()
    {
        _cls.Cancel();
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
}
