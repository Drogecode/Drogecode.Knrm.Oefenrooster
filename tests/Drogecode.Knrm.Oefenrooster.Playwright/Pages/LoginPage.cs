using Microsoft.Playwright;

namespace Drogecode.Knrm.Oefenrooster.Playwright.Pages;

public class LoginPage
{
    private readonly IPage _page;
    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task Login(string username, string password)
    {
        await _page.GotoAsync("https://localhost:44327/");
        await _page.Locator("id=username").FillAsync(username);
        await _page.Locator("id=password").FillAsync(password);
        await _page.Locator("id=kc-login").ClickAsync();
    }
}