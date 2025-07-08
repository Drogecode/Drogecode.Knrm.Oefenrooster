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
        await _page.FillAsync("input[name='username']", username);
        await _page.FillAsync("input[name='password']", password);
        await _page.Locator("id=kc-login").ClickAsync();
    }
}