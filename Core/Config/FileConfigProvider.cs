using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.Config;

public class FileConfigProvider : IConfigProvider
{
    private const string KeySeparator = ":";

    private readonly Cache _cache;
    private readonly TimeSpan _expireCacheItemAfter;
    private readonly string _targetDirectory;

    public FileConfigProvider(string targetDirectory, Cache cache, TimeSpan expireCacheItemAfter)
    {
        _targetDirectory = targetDirectory;
        _cache = cache;
        _expireCacheItemAfter = expireCacheItemAfter;
    }

    public void Dispose()
    {
        if (Directory.Exists(_targetDirectory)) Directory.Delete(_targetDirectory, true);

        _cache.Dispose();
    }

    public async Task<Result<T, Error>> Get<T>(string filePath, string key)
    {
        filePath = $"{_targetDirectory}/{filePath}";
        Result<object, Empty> cacheResult = _cache.TryGetValue(filePath);

        JObject? configObject = null;

        if (cacheResult.IsOk) configObject = cacheResult.Unwrap() as JObject;

        if (configObject is null)
        {
            if (!File.Exists(filePath))
                return Result<T, Error>.Err(new Error(ErrorKind.NotFound,
                    $"could not find file: {filePath}"));

            string configYaml = await File.ReadAllTextAsync(filePath);
            IDeserializer deserializer =
                new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            object? yamlObject = deserializer.Deserialize(configYaml);

            if (yamlObject is null)
                return Result<T, Error>.Err(new Error(ErrorKind.InvalidFileContent,
                    "invalid file content"));

            // YAML conversion to JSON because Microconfig works with YAML.
            ISerializer serializer = new SerializerBuilder().JsonCompatible().Build();

            string json = serializer.Serialize(yamlObject);

            configObject = JObject.Parse(json);

            _cache.Set(filePath, configObject, _expireCacheItemAfter);
        }

        string[] subKeys = key.Split(KeySeparator);
        
        Result<T, Error> valueResult = GetValue<T>(subKeys, configObject);

        if (!valueResult.IsOk) return Result<T, Error>.Err(valueResult.UnwrapErr());

        return Result<T, Error>.Ok(valueResult.Unwrap());
    }

    public void CleanCache()
    {
        _cache.Clear();
    }

    private static Result<T, Error> GetValue<T>(IEnumerable<string> subKeys, JObject jsonObject)
    {
        JToken? token = jsonObject;
        foreach (string subKey in subKeys)
        {
            token = token[subKey];

            if (token is null)
                return Result<T, Error>.Err(new Error(ErrorKind.InvalidArguments,
                    $"expected a JToken for subKey '{subKey}' but got null instead"));
        }

        T? value;

        if (token is JObject || token is JArray)
            value = token.ToObject<T>();
        else
            value = token.Value<T>();

        if (value is null)
            return Result<T, Error>.Err(new Error(ErrorKind.InvalidArguments, "failed to get value"));

        return Result<T, Error>.Ok(value);
    }
}