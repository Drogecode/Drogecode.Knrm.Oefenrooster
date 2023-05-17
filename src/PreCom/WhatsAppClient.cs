using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.PreCom;

public class WhatsAppClient
{
    const string UrlBase = "https://graph.facebook.com/v16.0/119008671191114/messages";
    readonly HttpClient _httpClient;
    public WhatsAppClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public Task Authorization()
    {
        //ToDo
        return Task.CompletedTask;
    }
}
