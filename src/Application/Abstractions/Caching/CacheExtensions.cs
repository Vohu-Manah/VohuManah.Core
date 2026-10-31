namespace Application.Abstractions.Caching;

public static class CacheExtensions
{
    public static T Retrieve<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
    {
        return cacheManager.Retrieve(key, 60, acquire);
    }

    public static T Retrieve<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
    {
        if (cacheManager.IsSet(key))
        {
            return cacheManager.Retrieve<T>(key) ?? acquire();
        }

        T result = acquire();
        cacheManager.Store(key, result, cacheTime);
        return result;
    }
}



