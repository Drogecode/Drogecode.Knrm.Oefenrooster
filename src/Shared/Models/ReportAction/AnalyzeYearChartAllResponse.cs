namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

public class AnalyzeYearChartAllResponse : BaseMultipleResponse
{
    public List<AnalyzeYearDetails> Years { get; set; } = new List<AnalyzeYearDetails>();
}

public class AnalyzeYearDetails
{
    public int Year { get; set; }
    public List<AnalyzeMonthDetails> Months { get; set; } = new List<AnalyzeMonthDetails>();
}

public class AnalyzeMonthDetails
{
    public int Month { get; set; }
    public int Count { get; set; }
}