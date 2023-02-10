using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class DrogeCodeGlobal : RefreshModel
{
    public event Func<NewTraining, Guid, Task>? NewTrainingAddedAsync;

    public async Task CallNewTrainingAddedAsync(NewTraining arg, Guid newId)
    {
        var task = NewTrainingAddedAsync?.Invoke(arg, newId);
        if (task != null)
            await task;
    }
}
