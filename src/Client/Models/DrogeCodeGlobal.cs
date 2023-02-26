using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class DrogeCodeGlobal : RefreshModel
{
    public event Func<EditTraining, Task>? NewTrainingAddedAsync;

    public async Task CallNewTrainingAddedAsync(EditTraining arg)
    {
        var task = NewTrainingAddedAsync?.Invoke(arg);
        if (task != null)
            await task;
    }
}
