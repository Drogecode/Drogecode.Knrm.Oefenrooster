﻿namespace Drogecode.Knrm.Oefenrooster.Shared.Enums;

public enum SettingName
{
    None = 0,
    /// <summary>
    /// Type: bool
    /// </summary>
    TrainingToCalendar = 1,
    /// <summary>
    /// Type: string
    /// </summary>
    TimeZone = 2,
    /// <summary>
    /// Type: string
    /// </summary>
    CalendarPrefix = 3,
    /// <summary>
    /// Type: bool
    /// </summary>
    SyncPreComWithCalendar = 4,
    /// <summary>
    /// Type: string
    /// </summary>
    PreComAvailableText = 5,
    /// <summary>
    /// Type: bool
    /// </summary>
    SyncPreComDeleteOld = 6,
    /// <summary>
    /// Type: int
    /// </summary>
    PreComDaysInFuture = 7,
    /// <summary>
    /// Type: bool
    /// </summary>
    SyncPreComWithExternal = 8,
    /// <summary>
    /// Type: bool
    /// </summary>
    DelaySyncingTrainingToOutlook = 9
}