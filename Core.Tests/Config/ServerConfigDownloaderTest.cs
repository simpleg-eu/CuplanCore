using System.Net;
using Core;
using Core.Config;
using Moq;
using Xunit;

namespace Cuplan.Core.Tests.Config;

public class ServerConfigDownloaderTest : TestBase, IDisposable
{
    private string? _targetDirectory;

    public void Dispose()
    {
        if (Directory.Exists(_targetDirectory)) Directory.Delete(_targetDirectory, true);
    }

    [Fact]
    public async Task Download_ExpectedFile()
    {
        const string exampleUrl = "https://simpleg.eu";
        const string exampleComponent = "dummy";
        const string exampleFile = "example.zip";
        const string expectedFile = "application.yaml";
        const string mockExampleUrl = $"{exampleUrl}/get/{exampleComponent}";
        byte[] exampleFileBytes = await File.ReadAllBytesAsync($"{TestDataPath}/{exampleFile}");
        HttpContent content = new ByteArrayContent(exampleFileBytes);
        HttpResponseMessage response = new();
        response.StatusCode = HttpStatusCode.OK;
        response.Content = content;
        Mock<IHttpClient> httpClient = new();
        httpClient.Setup(h => h.GetAsync(mockExampleUrl))
            .ReturnsAsync(response);

        ServerConfigDownloader downloader = new(httpClient.Object, new ZipExtractor());

        Result<string, Error> result = await downloader.Download(exampleUrl, exampleComponent);

        Assert.True(result.IsOk);
        _targetDirectory = result.Unwrap();
        Assert.NotNull(_targetDirectory);
        Assert.True(File.Exists($"{_targetDirectory}/{expectedFile}"),
            $"Expected file '{expectedFile}' does not exist within target directory '{_targetDirectory}'.");
    }
}