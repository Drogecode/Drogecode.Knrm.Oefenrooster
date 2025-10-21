using System.Diagnostics;
using System.Text;
using Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Managers;

public class OutlookManager : DrogeManager, IOutlookManager
{
    private readonly IUserLinkedMailsService _userLinkedMailsService;
    private readonly ITrainingTypesService _trainingTypesService;
    private readonly IUserSettingService _userSettingService;
    private readonly IScheduleService _scheduleService;
    private readonly IGraphService _graphService;

    public OutlookManager(ILogger<OutlookManager> logger, 
        IUserLinkedMailsService userLinkedMailsService, 
        ITrainingTypesService trainingTypesService, 
        IUserSettingService userSettingService, 
        IScheduleService scheduleService, 
        IGraphService graphService) : base(logger)
    {
        _userLinkedMailsService = userLinkedMailsService;
        _trainingTypesService = trainingTypesService;
        _userSettingService = userSettingService;
        _scheduleService = scheduleService;
        _graphService = graphService;
    }


    public async Task ToOutlookCalendar(Guid planUserId, string? externalUserId, Guid? trainingId, bool assigned, TrainingAdvance? training, Guid currentUserId, Guid customerId, Guid? availableId,
        string? calendarEventId, string? functionName, bool fromBackgroundWorker, CancellationToken clt)
    {
        try
        {
            var allUserLinkedMail = (await _userLinkedMailsService.AllUserLinkedMail(30, 0, planUserId, customerId, true, clt)).UserLinkedMails ?? [];
            var delay = await _userSettingService.GetBoolUserSetting(customerId, currentUserId, SettingName.DelaySyncingTrainingToOutlook, false, clt);
            if (delay.Value && !fromBackgroundWorker && allUserLinkedMail.Any(x => x is { IsActive: true, IsEnabled: true, IsDrogeCodeUser: false }))
            {
                return;
            }

            if (assigned && (await _userSettingService.GetBoolUserSetting(customerId, planUserId, SettingName.TrainingToCalendar, false, clt)).Value)
            {
#if DEBUG
                // Be careful when debugging.
                Debugger.Break();
#endif
                if (training is null && trainingId is not null)
                    training = (await _scheduleService.GetTrainingById(planUserId, customerId, trainingId.Value, clt)).Training;
                var type = await _trainingTypesService.GetById(training?.RoosterTrainingTypeId ?? Guid.Empty, customerId, clt);
                var preText = await _userSettingService.GetStringUserSetting(customerId, planUserId, SettingName.CalendarPrefix, string.Empty, clt);
                var text = GetTrainingCalenderText(type.TrainingType?.Name, training?.Name, functionName, preText.Value);
                if (training is null)
                {
                    Logger.LogWarning("Failed to set a training for trainingId {trainingId}", trainingId);
                    return;
                }

                _graphService.InitializeGraph();
                if (string.IsNullOrEmpty(calendarEventId))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        Logger.LogInformation("Creating event for user `{userId}` for training `{trainingId}`", planUserId, trainingId);
                        var eventResult = await _graphService.AddToCalendar(externalUserId, text, training.DateStart, training.DateEnd, !training.ShowTime, FreeBusyStatus.Busy, allUserLinkedMail);
                        if (eventResult is not null)
                            await _scheduleService.PatchEventIdForUserAvailible(planUserId, customerId, availableId, eventResult.Id, clt);
                    }
                    else
                    {
                        Logger.LogWarning(
                            "text is null or empty for {customerId} {planUserId} {trainingId} {assigned} {currentUserId} {availableId} {calendarEventId}",
                            customerId, planUserId, trainingId, assigned, currentUserId, availableId, calendarEventId);
                    }
                }
                else
                {
                    Logger.LogInformation("Patching event for user `{userId}` for training `{trainingId}`", planUserId, trainingId);
                    await _graphService.PatchCalender(externalUserId, calendarEventId, text, training.DateStart, training.DateEnd, !training.ShowTime, FreeBusyStatus.Busy, allUserLinkedMail);
                    if (availableId is not null)
                    {
                        await _scheduleService.PatchLastSynced(customerId, planUserId, availableId.Value, clt);
                        await _scheduleService.SaveDb(clt);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(calendarEventId))
            {
                _graphService.InitializeGraph();
                await _graphService.DeleteCalendarEvent(externalUserId, calendarEventId, clt);
                await _scheduleService.PatchEventIdForUserAvailible(planUserId, customerId, availableId, null, clt);
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in ToOutlookCalendar {customerId} {planUserId} {trainingId} {assigned} {currentUserId} {availableId} {calendarEventId} and training is {trainingNullOrNot}",
                customerId, planUserId, trainingId, assigned, currentUserId, availableId, calendarEventId, training is null ? "null" : "not null");
        }
    }

    public static string GetTrainingCalenderText(string? trainingTypeName, string? trainingName, string? functionName, string preText)
    {
        var text = new StringBuilder();
        text.Append(preText);
        if (!string.IsNullOrEmpty(trainingTypeName))
            text.Append(trainingTypeName);
        if (!string.IsNullOrEmpty(trainingTypeName) && !string.IsNullOrEmpty(trainingName))
            text.Append(" - ");
        if (!string.IsNullOrEmpty(trainingName))
            text.Append(trainingName);
        if (!string.IsNullOrEmpty(functionName))
            text.Append(" * ").Append(functionName);
        return text.ToString();
    }
}