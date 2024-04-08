namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

public class DbLinkUserUser
{
    public Guid UserAId { get; set; }
    public Guid UserBId { get; set; }
    public UserUserLinkType LinkType { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }


    public DbUsers UserA { get; set; } = null!;
    public DbUsers UserB { get; set; } = null!;
}
