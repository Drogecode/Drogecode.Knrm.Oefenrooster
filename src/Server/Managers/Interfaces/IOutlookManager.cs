using Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;

public interface IOutlookManager : IDrogeManager
{
    Task ToOutlookCalendar(Guid planUserId, string? externalUserId, Guid? trainingId, bool assigned, TrainingAdvance? training, Guid currentUserId, Guid customerId, Guid? availableId,
        string? calendarEventId, string? functionName, bool fromBackgroundWorker, CancellationToken clt);
}