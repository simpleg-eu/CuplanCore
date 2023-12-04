namespace Core;

public interface IHttpClient : IDisposable
{
    public Task<HttpResponseMessage> GetAsync(string? url);
}