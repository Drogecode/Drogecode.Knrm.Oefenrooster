﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

public class DrogeAction : SharePointListBase
{
    public double? Number { get; set; }
    public string? ShortDescription { get; set; }
    public string? Prio { get; set; }
    public string? Request { get; set; }
    public string? ForTheBenefitOf { get; set; }
    public string? Causes { get; set; }
    public string? Implications { get; set; }
    public string? CallMadeBy { get; set; }
    public double? CountSailors { get; set; }
    public double? CountSaved { get; set; }
    public double? CountAnimals { get; set; }
    public string? Completedby { get; set; }
    public DateTime Departure { get; set; }
}