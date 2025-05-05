using Microsoft.Playwright;

namespace Drogecode.Knrm.Oefenrooster.Playwright.Pages;

public class LoginPage
{
    private readonly IPage _page;
    private string _baseUrl;
    public LoginPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task Login(string username, string password)
    {
        await _page.GotoAsync(_baseUrl);
        await _page.Locator("id=username").FillAsync(username);
        await _page.Locator("id=password").FillAsync(password);
        await _page.Locator("id=kc-login").ClickAsync();
    }
}