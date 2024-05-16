using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

public class SharePointAction : SharePointListBase
{
    public double? Number { get; set; }
    public string? ShortDescription { get; set; }
    public string? Prio { get; set; }
    public string? Type { get; set; }
    public string? Request { get; set; }
    public string? ForTheBenefitOf { get; set; }
    public string? Causes { get; set; }
    public string? Implications { get; set; }
    public string? Area { get; set; }
    public string? WindDirection { get; set; }
    public int? WindPower { get; set; }
    public double? WaterTemperature { get; set; }
    public double? GolfHight { get; set; }
    public int? Sight { get; set; }
    public string? WeatherCondition { get; set; }
    public string? CallMadeBy { get; set; }
    public double? CountSailors { get; set; }
    public double? CountSaved { get; set; }
    public double? CountAnimals { get; set; }
    public string? FunctioningMaterial { get; set; }
    public string? ProblemsWithWeed { get; set; }
    public string? Completedby { get; set; }
    public DateTime Departure { get; set; }
}