using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace Core.Config;

public class ServerConfigDownloader : IConfigDownloader
{
    private readonly IHttpClient _client;
    private readonly IExtractor _extractor;

    public ServerConfigDownloader(IHttpClient client, IExtractor extractor)
    {
        _client = client;
        _extractor = extractor;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<Result<string, Error>> Download(string url, string component)
    {
        try
        {
            url = $"{url}/get/{component}";
            string targetDirectory = $"{Directory.GetCurrentDirectory()}/{new Guid().ToString()}";
            Directory.CreateDirectory(targetDirectory);

            Result<Empty, Error> downloadResult = await DownloadConfig(url, targetDirectory);

            if (!downloadResult.IsOk) return Result<string, Error>.Err(downloadResult.UnwrapErr());

            return Result<string, Error>.Ok(targetDirectory);
        }
        catch (Exception e)
        {
            return Result<string, Error>.Err(new Error(ErrorKind.ExceptionThrown,
                $"an exception '{e.GetType().Name}' has been thrown: {e.Message}: {e.StackTrace}"));
        }
    }

    private async Task<Result<Empty, Error>> DownloadConfig(string url, string targetDirectory)
    {
        try
        {
            using HttpResponseMessage response = await _client.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
                return Result<Empty, Error>.Err(new Error(ErrorKind.DownloadFailure,
                    $"failed to download zip file containing configuration with url: {url}"));

            byte[] configZipData = await response.Content.ReadAsByteArrayAsync();
            
            return _extractor.Extract(configZipData, targetDirectory);
        }
        catch (Exception e)
        {
            return Result<Empty, Error>.Err(new Error(ErrorKind.DownloadFailure,
                $"an exception '{e.GetType().Name}' has been thrown while downloading configuration: {e.Message}: {e.StackTrace}"));
        }
    }
}