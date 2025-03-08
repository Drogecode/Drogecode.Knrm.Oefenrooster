using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Drogecode.Knrm.Oefenrooster.PreCom;

public class WhatsAppClient
{
    const string UrlBase = "https://graph.facebook.com/v22.0/119008671191114/messages";
    private readonly string _bearer;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public WhatsAppClient(HttpClient httpClient, string bearer, ILogger logger)
    {
        _httpClient = httpClient;
        _bearer = bearer;
        _logger = logger;
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task SendMessage(string to, string body)
    {
        using (_httpClient)
        {
            var message = new WhatsAppMessage()
            {
                to = to,
                text = new WhatsAppMessageText()
                {
                    body = body
                }
            };
            var messageAsString = System.Text.Json.JsonSerializer.Serialize(message);
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), UrlBase))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearer);
                request.Content = new StringContent(messageAsString);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await _httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("whatsapp result: {result}", result);
            }
        }
    }

    private struct WhatsAppMessage
    {
        public WhatsAppMessage()
        {
            to = "";
            text = new WhatsAppMessageText();
        }

        public bool preview_url { get; set; } = true;
        public string messaging_product { get; set; } = "whatsapp";
        public string recipient_type { get; set; } = "individual";
        public string to { get; set; }
        public string type { get; set; } = "text";
        public WhatsAppMessageText text { get; set; }
    }

    private struct WhatsAppMessageText
    {
        public string body { get; set; }
    }
}