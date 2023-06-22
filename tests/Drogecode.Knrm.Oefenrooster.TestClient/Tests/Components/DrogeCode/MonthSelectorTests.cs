using Bunit;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using FluentAssertions;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode;

public class MonthSelectorTests : TestContext
{
    [Fact]
    public void MonthThisYearTest()
    {
        var month = DateTime.Today;
        var cut = RenderComponent<MonthSelector>(parameter => parameter.Add(p => p.Month, month));
        cut.Markup.Should().Contain(month.ToString("MMMM"));
        cut.Markup.Should().NotContain(month.Year.ToString());
    }

    [Fact]
    public void MonthNotThisYearTest()
    {
        var month = new DateTime(2022, 11, 5);
        var cut = RenderComponent<MonthSelector>(parameter => parameter.Add(p => p.Month, month));
        cut.Markup.Should().Contain(month.ToString("MMMM"));
        cut.Markup.Should().Contain(month.Year.ToString());
    }
}
