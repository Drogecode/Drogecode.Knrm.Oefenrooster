using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class DrogeCodeGlobal : RefreshModel
{
    public event Func<EditTraining, Task>? NewTrainingAddedAsync;
    public event Func<Guid, Task>? TrainingDeletedAsync;
    public event Func<bool, Task>? DarkLightChangedAsync;
    public event Func<Task>? VisibilityChangeAsync;

    private bool _isDarkMode;
    private DateTime _lastVisibilityChange = DateTime.UtcNow;

    public async Task CallNewTrainingAddedAsync(EditTraining arg)
    {
        var task = NewTrainingAddedAsync?.Invoke(arg);
        if (task != null)
            await task;
    }
    public async Task CallTrainingDeletedAsync(Guid id)
    {
        var task = TrainingDeletedAsync?.Invoke(id);
        if (task != null)
            await task;
    }
    public async Task CallDarkLightChangedAsync(bool arg)
    {
        if (_isDarkMode == arg) return;
        _isDarkMode = arg;
        var task = DarkLightChangedAsync?.Invoke(arg);
        if (task != null)
            await task;
    }
    public async Task CallVisibilityChangeAsync()
    {
        if (_lastVisibilityChange.AddMinutes(5).CompareTo(DateTime.UtcNow) > 0)
            return;
        _lastVisibilityChange = DateTime.UtcNow;
        var task = VisibilityChangeAsync?.Invoke();
        if (task != null)
            await task;
    }
}
