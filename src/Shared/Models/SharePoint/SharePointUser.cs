using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class SharePointUser : ICloneable
{
    public string? SharePointID { get; set; }
    public Guid? DrogeCodeId { get; set; }
    public string? Name { get; set; }
    public SharePointRole Role { get; set; }
    public int Order { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}