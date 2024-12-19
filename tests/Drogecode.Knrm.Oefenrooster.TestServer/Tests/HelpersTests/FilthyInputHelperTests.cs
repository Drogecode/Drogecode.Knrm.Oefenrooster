using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Microsoft.Extensions.Logging;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.HelpersTests;

public class FilthyInputHelperTests(ILoggerFactory loggerFactory)
{
    [Fact]
    public void CleanListNoIssues()
    {
        var body = new List<string> { "Clean list no issues" };
        var cleaned = FilthyInputHelper.CleanList(body, 1, loggerFactory.CreateLogger("FilthyInputHelperTests"));
        Assert.NotNull(cleaned);
        Assert.NotEmpty(cleaned);
        cleaned.Should().HaveCount(1);
    }

    [Fact]
    public void CleanListToMannyValues()
    {
        var CORRECT_VALUE = "Clean list to manny values";
        var WRONG_VALUE_1 = "this one is to much";
        var body = new List<string> { CORRECT_VALUE, WRONG_VALUE_1 };
        var cleaned = FilthyInputHelper.CleanList(body, 1, loggerFactory.CreateLogger("FilthyInputHelperTests"));
        Assert.NotNull(cleaned);
        Assert.NotEmpty(cleaned);
        cleaned.Should().HaveCount(1);
        cleaned.Should().Contain(CORRECT_VALUE);
        cleaned.Should().NotContain(WRONG_VALUE_1);
    }

    [Fact]
    public void CleanListContainsNumber()
    {
        // numbers are allowed
        var body = new List<string> { "first has no number", "I have 1 number", "third has no number" };
        var cleaned = FilthyInputHelper.CleanList(body, 3, loggerFactory.CreateLogger("FilthyInputHelperTests"));
        Assert.NotNull(cleaned);
        Assert.NotEmpty(cleaned);
        cleaned.Should().HaveCount(3);
    }

    [Fact]
    public void CleanListWithIssues()
    {
        var CORRECT_VALUE = "No issue";
        var WRONG_VALUE_1 = "This $ is not allowed";
        var WRONG_VALUE_2 = "This % is also not allowed";
        var body = new List<string> { CORRECT_VALUE, WRONG_VALUE_1, WRONG_VALUE_2 };
        var cleaned = FilthyInputHelper.CleanList(body, 3, loggerFactory.CreateLogger("FilthyInputHelperTests"));
        Assert.NotNull(cleaned);
        Assert.NotEmpty(cleaned);
        cleaned.Should().HaveCount(1);
        cleaned.Should().Contain(CORRECT_VALUE);
        cleaned.Should().NotContain(WRONG_VALUE_1);
        cleaned.Should().NotContain(WRONG_VALUE_2);
    }

    [Fact]
    public void CleanListWithIssuesNoCorrectValues()
    {
        var WRONG_VALUE_1 = "This $ is not allowed";
        var WRONG_VALUE_2 = "This % is also not allowed";
        var body = new List<string> { WRONG_VALUE_1, WRONG_VALUE_2 };
        var cleaned = FilthyInputHelper.CleanList(body, 3, loggerFactory.CreateLogger("FilthyInputHelperTests"));
        Assert.Null(cleaned);
    }

    [Fact]
    public void CleanListTrimValues()
    {
        var body = new List<string> { "Clean list with trailing white space " };
        var cleaned = FilthyInputHelper.CleanList(body, 1, loggerFactory.CreateLogger("FilthyInputHelperTests"));
        Assert.NotNull(cleaned);
        Assert.NotEmpty(cleaned);
        cleaned.Should().HaveCount(1);
        cleaned.Should().Contain("Clean list with trailing white space");
    }
}