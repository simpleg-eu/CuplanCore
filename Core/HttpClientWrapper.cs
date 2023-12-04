using System.Net.Http.Headers;

namespace Core;

public class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _httpClient = new();

    public string AuthorizationBearerToken
    {
        set => _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public Task<HttpResponseMessage> GetAsync(string? url)
    {
        return _httpClient.GetAsync(url);
    }
}