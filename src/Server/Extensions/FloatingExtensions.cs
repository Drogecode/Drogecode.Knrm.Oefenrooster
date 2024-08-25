namespace Drogecode.Knrm.Oefenrooster.Server.Extensions;

public static class FloatingExtensions
{
    public static bool FloatingEquals(this double value, double other, double tolerance) => (Math.Abs(value - other) <= tolerance);
}