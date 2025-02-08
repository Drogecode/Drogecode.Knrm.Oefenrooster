namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class DrogeCodeGlobal : RefreshModel
{
    public event Func<EditTraining, Task>? NewTrainingAddedAsync;
    public event Func<Guid, Task>? TrainingDeletedAsync;
    public event Func<bool, Task>? DarkLightChangedAsync;
    public event Func<Task>? VisibilityChangeAsync;

    public bool DarkMode { get; set; }

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
        if (!DarkMode.Equals(arg))
        {
            DebugHelper.WriteLine("DarkMode and arg should be equal");
            DarkMode = arg;
        }
        var task = DarkLightChangedAsync?.Invoke(arg);
        if (task != null)
            await task;
    }
    public async Task CallVisibilityChangeAsync()
    {
        var task = VisibilityChangeAsync?.Invoke();
        if (task != null)
            await task;
    }
}
