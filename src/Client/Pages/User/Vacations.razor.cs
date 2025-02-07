using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.User;

public sealed partial class Vacations : IDisposable
{
    [Inject] private IStringLocalizer<Vacations> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private HolidayRepository _holidayRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<Holiday>? _holidays { get; set; }
    private RefreshModel _refreshModel = new();
    private bool _updating;
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;

    protected override async Task OnParametersSetAsync()
    {
        _holidays = await _holidayRepository.GetAll(_cls.Token);
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private void GoToVacationAdd(Holiday? holiday = null)
    {
        if (holiday is null)
        {
            Navigation.NavigateTo("/user/vacations/add");
        }
        else
        {
            Navigation.NavigateTo($"/user/vacations/edit/{holiday.Id}");
        }
    }

    private async Task Delete(Holiday? holiday)
    {
        var response = await _holidayRepository.Delete(holiday!.Id, _cls.Token);
        if (response != null && response.Success)
        {
            await RefreshMeAsync();
        }
    }

    private async Task RefreshMeAsync()
    {
        _holidays = await _holidayRepository.GetAll(_cls.Token);
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
    }
}
