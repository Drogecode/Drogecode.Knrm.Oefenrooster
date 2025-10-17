namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class UserMonthInfo : ICloneable
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int All { get; set; }
    public int Valid { get; set; }
    public object Clone()
    {
        return MemberwiseClone();
    }
}
