namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class RefreshModel
{
    public event Action? RefreshRequested;
    public event Func<Task>? RefreshRequestedAsync;

    public void CallRequestRefresh()
    {
        RefreshRequested?.Invoke();
        RefreshRequestedAsync?.Invoke();
    }

    public async Task CallRequestRefreshAsync()
    {
        RefreshRequested?.Invoke();
        var task = RefreshRequestedAsync?.Invoke();
        if (task != null)
            await task;
    }
}