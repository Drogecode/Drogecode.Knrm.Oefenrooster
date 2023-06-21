using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
