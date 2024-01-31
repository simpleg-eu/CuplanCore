using Core;
using Core.Config;
using Moq;
using Xunit;

namespace Cuplan.Core.Tests.Config;

public class ClientTest : TestBase
{
    private const string ExpectedValue = "1234abcd";
    private const string Host = "https://simpleg.eu";
    private const string Stage = "dummy";
    private const string Environment = "development";
    private const string Component = "dummy";
    private const double DownloadAgainAfterSeconds = 1;
    private const string FilePath = "application.yaml";
    private const string ConfigKey = "Parent:Child";

    private readonly string _workingPath = Guid.NewGuid().ToString();
    private readonly byte[] _packageData = [];
    private readonly Mock<IDownloader> _downloaderMock;
    private readonly Mock<IExtractor> _extractorMock;
    private readonly Mock<IProvider> _providerMock;
    private readonly Client _client;

    public ClientTest()
    {
        _downloaderMock = new(MockBehavior.Strict);
        _downloaderMock.Setup(d => d.Download(
                It.Is<string>(h => h == Host),
                It.Is<string>(s => s == Stage),
                It.Is<string>(e => e == Environment),
                It.Is<string>(c => c == Component)))
            .Returns(Task.FromResult(Result<byte[], Error>.Ok(_packageData)));
        _extractorMock = new(MockBehavior.Strict);
        _extractorMock.Setup(e => e.Extract(It.Is<byte[]>(p => p.Equals(_packageData)), It.IsAny<string>()))
            .Returns(Result<Empty, Error>.Ok(new Empty()));
        _providerMock = new(MockBehavior.Strict);
        _providerMock.Setup(c =>
                c.Get<string>(It.Is<string>(f => f == FilePath), It.Is<string>(k => k == ConfigKey)))
            .ReturnsAsync(Result<string, Error>.Ok(ExpectedValue));
        _providerMock.Setup(c => c.CleanCache());
        _client = new(Host, Stage, Environment, Component, _workingPath, DownloadAgainAfterSeconds,
            _downloaderMock.Object, _extractorMock.Object, _providerMock.Object);
    }
    
    [Fact]
    public async Task Get_EmptyWorkingPath_DownloadsConfiguration()
    {
        Result<string, Error> result = await _client.Get<string>(FilePath, ConfigKey);
        
        _client.Dispose();
        AssertExpectedValue(result);
        AssertCompleteFlowExecutedTimes(1);
    }

    [Fact]
    public async Task Get_AfterDownloadAgain_Downloads()
    {
        await _client.Get<string>(FilePath, ConfigKey);
        Thread.Sleep(1500);

        Result<string, Error> result = await _client.Get<string>(FilePath, ConfigKey);
        
        _client.Dispose();
        AssertExpectedValue(result);
        AssertCompleteFlowExecutedTimes(2);
    }

    [Fact]
    public void Dispose_RemovesWorkingPath()
    {
        Directory.CreateDirectory(_workingPath);
        
        _client.Dispose();
        
        Assert.False(Directory.Exists(_workingPath));
    }

    private void AssertExpectedValue(Result<string, Error> result)
    {
        Assert.True(result.IsOk, "Expected result to be ok in order to extract value.");
        Assert.Equal(ExpectedValue, result.Unwrap());
    }

    private void AssertCompleteFlowExecutedTimes(int times)
    {
        _downloaderMock.Verify(d => d.Download(
            It.Is<string>(h => h == Host),
            It.Is<string>(s => s == Stage),
            It.Is<string>(e => e == Environment),
            It.Is<string>(c => c == Component)), Times.Exactly(times));
        _extractorMock.Verify(e => e.Extract(It.Is<byte[]>(p => p.Equals(_packageData)), It.IsAny<string>()), Times.Exactly(times));
        _providerMock.Verify(p => p.CleanCache(), Times.Exactly(times));
        _providerMock.Verify(p => p.Get<string>(It.Is<string>(f => f == FilePath), It.Is<string>(k => k == ConfigKey)), Times.Exactly(times));
    }
}