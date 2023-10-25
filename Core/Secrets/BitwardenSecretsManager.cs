using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Secrets;

internal struct BitwardenSecret
{
    public string id;
    public string organizationId;
    public string projectId;
    public string key;
    public string value;
    public string creationDate;
    public string revisionDate;
}

public class BitwardenSecretsManager : ISecretsManager
{
    public static readonly string AccessTokenEnvVar = "SECRETS_MANAGER_ACCESS_TOKEN";

    private readonly string? _accessToken;
    private readonly ILogger<BitwardenSecretsManager>? _logger;

    public BitwardenSecretsManager(ILogger<BitwardenSecretsManager>? logger)
    {
        _logger = logger;
        _accessToken = Environment.GetEnvironmentVariable(AccessTokenEnvVar);
    }

    public string? get(string secretId)
    {
        if (_accessToken is null)
        {
            _logger?.LogTrace("tried to get secret with no access token");
            return null;
        }

        try
        {
            Process bws = new();
            bws.StartInfo.FileName = "bws";
            bws.StartInfo.Arguments = $"secret get {secretId} --access-token {_accessToken}";
            bws.StartInfo.RedirectStandardOutput = true;
            bws.Start();

            StreamReader output = bws.StandardOutput;

            string serializedSecret = output.ReadToEnd();

            BitwardenSecret secret = JsonConvert.DeserializeObject<BitwardenSecret>(serializedSecret);

            return secret.value;
        }
        catch (Exception e)
        {
            _logger?.LogWarning($"failed to get secret: {e}");
            return null;
        }
    }
}