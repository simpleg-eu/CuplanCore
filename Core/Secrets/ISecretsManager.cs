namespace Core.Secrets;

public interface ISecretsManager
{
    /// <summary>
    /// Retrieves a secret from the secret manager by its id.
    /// </summary>
    /// <param name="secretId">The id of the secret.</param>
    /// <returns>The secret if it is found, null otherwise.</returns>
    string? Get(string secretId);
}