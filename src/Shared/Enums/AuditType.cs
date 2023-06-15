namespace Drogecode.Knrm.Oefenrooster.Shared.Enums;

public enum AuditType
{
    None = 0,
    DataBaseUpgrade = 1,
    AddUser = 2,
    AddTraining = 3,
    PatchTraining = 4,
    SyncAllUsers = 5,
    PatchAssignedUser = 6,
    CatchAll = 7,
    PreComRaw = 8
}
