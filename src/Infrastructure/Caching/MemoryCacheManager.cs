using System;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Infrastructure.Caching;

public sealed class MemoryCacheManager : Application.Abstractions.Caching.ICacheManager
{
    private readonly ObjectCache _cache = MemoryCache.Default;

    public T? Retrieve<T>(string key)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        object? item = _cache[key];
        return item is T typedItem ? typedItem : default;
    }

    public void Store(string key, object? data, int cacheTime)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        if (cacheTime <= 0)
        {
            throw new ArgumentException("زمان کش باید بزرگتر از صفر باشد", nameof(cacheTime));
        }
        if (data is null)
        {
            return;
        }

        var policy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(cacheTime)
        };
        _cache.Set(key, data, policy);
    }

    public bool IsSet(string key)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        return _cache.Contains(key);
    }

    public void Remove(string key)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        _cache.Remove(key);
    }

    public void RemoveByPattern(string pattern)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(pattern);

        var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        var keysToRemove = _cache
            .Where(item => regex.IsMatch(item.Key))
            .Select(item => item.Key)
            .ToList();

        foreach (string key in keysToRemove)
        {
            _cache.Remove(key);
        }
    }

    public void Clear()
    {
        var keys = _cache.Select(item => item.Key).ToList();
        foreach (string key in keys)
        {
            _cache.Remove(key);
        }
    }
}
