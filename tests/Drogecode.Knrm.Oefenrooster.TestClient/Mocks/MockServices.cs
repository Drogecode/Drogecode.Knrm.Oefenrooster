using AutoFixture;
using Blazored.SessionStorage;
using Bunit;
using Bunit.TestDoubles;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Client.Services;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks;

public static class MockServices
{
    public static MockHttpMessageHandler AddMockServices(this TestServiceProvider services)
    {
        var mockHttpHandler = new MockHttpMessageHandler();
        var httpClient = mockHttpHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost");
        services.AddSingleton<HttpClient>(httpClient);
        return mockHttpHandler;
    }
}