using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;

public sealed partial class ForwardList : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<ForwardList>? L { get; set; }
    [Inject, NotNull] private PreComRepository? PreComRepository { get; set; }
    private CancellationTokenSource _cls = new();
    private MultiplePreComForwardsResponse? _forwards;
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

    public void Dispose()
    {
        _cls.Cancel();
    }
}