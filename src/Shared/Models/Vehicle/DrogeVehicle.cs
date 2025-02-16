namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

public class DrogeVehicle
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
