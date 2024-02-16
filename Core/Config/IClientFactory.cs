namespace Core.Config;

public interface IClientFactory
{
    /// <summary>
    ///     Builds a <see cref="Client" /> instance.
    /// </summary>
    /// <param name="accessToken">Access token to be used to access the configuration server.</param>
    /// <param name="host">The URL of the configuration server.</param>
    /// <param name="stage">Stage of the configuration to be retrieved.</param>
    /// <param name="environment">Environment of the configuration to be retrieved.</param>
    /// <param name="component">Component, aka microservice, whose configuration wants to be retrieved.</param>
    /// <param name="workingPath">Path where the configuration will be stored.</param>
    /// <param name="downloadTimeoutInMilliseconds">Timeout download after specified milliseconds.</param>
    /// <returns><see cref="Client" /> if it is successful, or an <see cref="Error" /> otherwise.</returns>
    public Result<Client, Error> Build(string accessToken, string host, string stage, string environment,
        string component,
        string workingPath, int downloadTimeoutInMilliseconds);
}