using System.Collections.Concurrent;

namespace Core;

public class Cache : IDisposable
{
    private readonly ConcurrentDictionary<string, CacheItem<object>> _cache = new();
    private readonly Timer _timer;

    public Cache(TimeSpan cleanEveryTime)
    {
        _timer = new Timer(state =>
        {
            IList<string> keysToBeRemoved = new List<string>();
            foreach (string key in _cache.Keys)
            {
                CacheItem<object>? value;
                _cache.TryGetValue(key, out value);

                if (value is null) continue;

                if (value.IsExpired()) keysToBeRemoved.Add(key);
            }

            foreach (string key in keysToBeRemoved) _cache.TryRemove(key, out _);
        }, null, cleanEveryTime, cleanEveryTime);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    public bool IsEmpty => _cache.IsEmpty;

    public void Set(string key, object value, TimeSpan expirationTime)
    {
        DateTimeOffset expiration = DateTimeOffset.UtcNow.Add(expirationTime);
        CacheItem<object> cacheItem = new(value, expiration);

        _cache[key] = cacheItem;
    }

    public Result<object, Empty> TryGetValue(string key)
    {
        if (_cache.TryGetValue(key, out CacheItem<object>? cacheItem))
        {
            if (cacheItem.IsExpired())
            {
                _cache.TryRemove(key, out _);
                return Result<object, Empty>.Err(new Empty());
            }

            object value = cacheItem.Value;

            return Result<object, Empty>.Ok(value);
        }

        return Result<object, Empty>.Err(new Empty());
    }

    public bool HasKey(string key)
    {
        return _cache.ContainsKey(key);
    }

    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    private class CacheItem<T>
    {
        public CacheItem(T value, DateTimeOffset expiration)
        {
            Value = value;
            Expiration = expiration;
        }

        public T Value { get; }
        public DateTimeOffset Expiration { get; }

        public bool IsExpired()
        {
            return DateTimeOffset.UtcNow > Expiration;
        }
    }
}