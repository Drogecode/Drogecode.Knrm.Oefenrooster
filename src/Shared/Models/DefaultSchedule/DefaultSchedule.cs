using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class DefaultSchedule
{
    public Guid Id { get; set; }
    public Guid? RoosterTrainingTypeId { get; set; }
    public List<Guid>? VehicleIds { get; set; }
    public DayOfWeek WeekDay { get; set; }
    public TimeSpan? TimeStart { get; set; }
    public TimeSpan? TimeEnd { get; set; }
    public DateTime? ValidFromDefault { get; set; }
    public DateTime? ValidUntilDefault { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public string? Name { get; set; }
    public bool CountToTrainingTarget { get; set; } = true;
    public int Order { get; set; }
    public bool ShowTime { get; set; } = true;
    public List<DefaultUserSchedule>? UserSchedules { get; set; }
}

public class DefaultUserSchedule : ICloneable
{
    public Guid? UserDefaultAvailableId { get; set; }
    public Guid? GroupId { get; set; }
    public Availabilty? Available { get; set; }//ToDo Remove when all users on v0.3.50 or above
    public Availability? Availability { get; set; }
    public DateTime? ValidFromUser { get; set; }
    public DateTime? ValidUntilUser { get; set; }
    public bool Assigned { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class PatchDefaultUserSchedule : DefaultUserSchedule
{
    public Guid DefaultId { get; set; }
}
