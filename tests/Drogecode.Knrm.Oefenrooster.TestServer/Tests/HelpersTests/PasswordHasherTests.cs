using Drogecode.Knrm.Oefenrooster.Server.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.HelpersTests;

public class PasswordHasherTests
{
    [Theory]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("Y~xF[.)XW'$/E3kt}wj9Mc")]
    [InlineData("FBjQe{)?$@s#8tZqL3fr`H")]
    [InlineData("hvdybXe7Jkgs6}.ZWfx4;c")]
    [InlineData("x$Xv](W7Em>q:U3*,Lf+a)")]
    [InlineData("wvQnZ,TG/spt#MSH9]m;dA")]
    [InlineData("shP`k.[05$v(*vyRFBX/6-")]
    [InlineData("DkH7Po3IFnm,#[`NM5l\"To")]
    [InlineData("Q@9Ki'bH+BgKhjWxAk\"97G")]
    [InlineData("NE7;n360m}?ysN!4,?;YyN")]
    [InlineData("g%DM5LFog.]^%1+\"oCP7Ka")]
    [InlineData("testY~xF[.)XW'$/E3kt}wj9McFBjQe{)?$@s#8tZqL3fr`HhvdybXe7Jkgs6}.ZWfx4;cx$Xv](W7Em>q:U3*,Lf+a)wvQnZ,TG/spt#MSH9]m;dAshP`k.[05$v(*vyRFBX/6-DkH7Po3IFnm,#[`NM5l\"ToQ@9Ki'bH+BgKhjWxAk\"97GNE7;n360m}?ysN!4,?;YyNg%DM5LFog.]^%1+\"oCP7Ka")]
    public async Task PasswordHasherTest(string password)
    {
        var hashed = PasswordHasher.HashNewPassword(password);
        Assert.NotEmpty(hashed);
        Assert.NotEqual(password, hashed);
        var compare  = PasswordHasher.ComparePassword(password, hashed);
        Assert.True(compare);
        var hashed2 = PasswordHasher.HashNewPassword(password);
        Assert.NotEmpty(hashed2);
        Assert.NotEqual(hashed2, hashed);
    }
}