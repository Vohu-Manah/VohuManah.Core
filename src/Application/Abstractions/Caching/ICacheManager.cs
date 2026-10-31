namespace Application.Abstractions.Caching;

public interface ICacheManager
{
    T? Retrieve<T>(string key);

    void Store(string key, object? data, int cacheTime);

    bool IsSet(string key);

    void Remove(string key);

    void RemoveByPattern(string pattern);

    void Clear();
}

