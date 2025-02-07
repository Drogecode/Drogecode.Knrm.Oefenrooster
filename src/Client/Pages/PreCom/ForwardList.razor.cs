using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;

public sealed partial class ForwardList : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<ForwardList>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private PreComRepository? PreComRepository { get; set; }
    [Inject, NotNull] private IPreComClient? PreComClient { get; set; }
    private CancellationTokenSource _cls = new();
    private MultiplePreComForwardsResponse? _forwards;

    private readonly DialogOptions _editOptions = new()
    {
        MaxWidth = MaxWidth.Large,
        FullWidth = true
    };

    private bool _busy;
    private int _currentPage = 1;
    private readonly int _count = 30;
    private int _skip = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _forwards = await PreComRepository.GetAllForwards(_count, _skip, _cls.Token);
            StateHasChanged();
        }
    }

    private async Task Next(int nextPage)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        _skip = (nextPage - 1) * _count;
        _forwards = await PreComRepository.GetAllForwards(_count, _skip, _cls.Token);
        _busy = false;
        StateHasChanged();
    }

    // events
    void StartedEditingItem(PreComForward item)
    {
        DebugHelper.WriteLine($"Event = StartedEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    void CanceledEditingItem(PreComForward item)
    {
        DebugHelper.WriteLine($"Event = CanceledEditingItem, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    async Task CommittedItemChanges(PreComForward item)
    {
        bool success = false;
        if (item.CreatedOn is null)
        {
            if (_forwards?.TotalCount < 5)
            {
                success = (await PreComClient.PutForwardAsync(item)).Success;
            }
        }
        else
        {
            success = (await PreComClient.PatchForwardAsync(item)).Success;
        }

        if (success)
        {
            _forwards = await PreComRepository.GetAllForwards(_count, _skip, _cls.Token);
        }

        DebugHelper.WriteLine($"Event = CommittedItemChanges, Data = {System.Text.Json.JsonSerializer.Serialize(item)}");
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}