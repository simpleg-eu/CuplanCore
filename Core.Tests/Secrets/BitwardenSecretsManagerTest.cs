using Core.Secrets;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cuplan.Core.Tests.Secrets;

public class BitwardenSecretsManagerTestFixture
{
    public BitwardenSecretsManagerTestFixture()
    {
        Mock<ILogger<BitwardenSecretsManager>> logger = new(MockBehavior.Loose);

        SecretsManager = new BitwardenSecretsManager(logger.Object);
    }

    public BitwardenSecretsManager SecretsManager { get; }
}

public class BitwardenSecretsManagerTest
    (BitwardenSecretsManagerTestFixture fixture) : IClassFixture<BitwardenSecretsManagerTestFixture>
{
    public BitwardenSecretsManager SecretsManager { get; } = fixture.SecretsManager;

    [Fact]
    public void Get_TestSecret_ReturnsExpectedSecret()
    {
        const string testSecret = "7c1d5dfd-a58b-47cf-bee5-b0a600fe50c9";
        const string expectedSecret = "le_secret :)";


        string? secret = SecretsManager.get(testSecret);


        Assert.Equal(expectedSecret, secret);
    }

    [Fact]
    public void Get_NonExistingSecret_ReturnsNull()
    {
        const string nonExistingSecret = "7c1d5dfd-a58b-47cf-bee5-b0a600fe50c8";


        string? secret = SecretsManager.get(nonExistingSecret);


        Assert.Null(secret);
    }
}