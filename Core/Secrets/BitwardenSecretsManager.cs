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

public class BitwardenSecretsManager(ILogger<BitwardenSecretsManager> logger, string accessToken)
    : ISecretsManager
{
    public static readonly string AccessTokenEnvVar = "SECRETS_MANAGER_ACCESS_TOKEN";

    public string? get(string secretId)
    {
        try
        {
            Process bws = new();
            bws.StartInfo.FileName = "bws";
            bws.StartInfo.Arguments = $"secret get {secretId} --access-token {accessToken}";
            bws.StartInfo.RedirectStandardOutput = true;
            bws.Start();

            StreamReader output = bws.StandardOutput;

            string serializedSecret = output.ReadToEnd();

            BitwardenSecret secret = JsonConvert.DeserializeObject<BitwardenSecret>(serializedSecret);

            return secret.value;
        }
        catch (Exception e)
        {
            logger.LogWarning($"failed to get secret: {e}");
            return null;
        }
    }
}