using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class SharePointListBase
{
    public Guid Id { get; set; }
    public DateTime LastUpdated { get; set; }
    public string? OdataEtag { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Boat { get; set; }
    public string? Area { get; set; }
    public string? WindDirection { get; set; }
    public int? WindPower { get; set; }
    public double? WaterTemperature { get; set; }
    public double? GolfHight { get; set; }
    public int? Sight { get; set; }
    public string? WeatherCondition { get; set; }
    public string? FunctioningMaterial { get; set; }
    public string? ProblemsWithWeed { get; set; }
    public DateTime Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime Commencement { get; set; }
    public DateTime End { get; set; }
    public List<SharePointUser>? Users { get; set; }
}
