using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class DrogeCodeGlobal : RefreshModel
{
    public event Func<EditTraining, Task>? NewTrainingAddedAsync;
    public event Func<bool, Task>? DarkLightChangedAsync;

    private bool _isDarkMode;

    public async Task CallNewTrainingAddedAsync(EditTraining arg)
    {
        var task = NewTrainingAddedAsync?.Invoke(arg);
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
}
