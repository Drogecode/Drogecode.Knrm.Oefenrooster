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
        await _page.FillAsync("input[name='username']", username);
        await _page.FillAsync("input[name='password']", password);
        await _page.Locator("id=kc-login").ClickAsync();
    }
    
    public async Task Logout()
    {
        await _page.GetByTestId("top-menu-settings").Locator("button").ClickAsync();
        await _page.GetByTestId("user-logout").ClickAsync();
    }
}