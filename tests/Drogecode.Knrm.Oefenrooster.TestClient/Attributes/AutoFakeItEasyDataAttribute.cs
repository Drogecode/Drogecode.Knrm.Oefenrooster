using AutoFixture;
using AutoFixture.AutoFakeItEasy;

// https://github.com/bUnit-dev/bUnit/discussions/239
namespace Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AutoFakeItEasyDataAttribute : AutoDataAttribute
{
    public AutoFakeItEasyDataAttribute()
        : base(FixtureFactory)
    {
    }

    private static IFixture FixtureFactory()
    {
        return new Fixture().Customize(new AutoFakeItEasyCustomization());
    }
}
