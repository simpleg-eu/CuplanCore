namespace Core;

public class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _httpClient = new();

    public Task<HttpResponseMessage> GetAsync(string? url)
    {
        return _httpClient.GetAsync(url);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}