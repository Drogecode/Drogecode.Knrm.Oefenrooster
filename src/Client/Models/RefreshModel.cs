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

    public Task CallRequestRefreshAsync()
    {
        RefreshRequested?.Invoke();
        var task = RefreshRequestedAsync?.Invoke();
        return task ?? Task.CompletedTask;
    }
}