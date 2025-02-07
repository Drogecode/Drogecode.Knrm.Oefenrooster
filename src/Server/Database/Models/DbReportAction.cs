using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("ReportActions")]
public class DbReportAction
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? OdataEtag { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime Commencement { get; set; }
    public DateTime Departure { get; set; }
    public DateTime Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public double? Number { get; set; }
    public string? ShortDescription { get; set; }
    public string? Prio { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
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
    public string? Boat { get; set; }
    public string? FunctioningMaterial { get; set; }
    public string? ProblemsWithWeed { get; set; }
    public string? Completedby { get; set; }
    public double TotalMinutes { get; set; }
    public int TotalFullHours { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbReportUser>? Users { get; set; }
}
