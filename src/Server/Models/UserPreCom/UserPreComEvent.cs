namespace Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;

public class UserPreComEvent
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? CalendarEventId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsFullDay { get; set; }
}