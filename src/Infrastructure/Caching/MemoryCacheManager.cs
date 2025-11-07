using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

using Application.Abstractions.Caching;

namespace Infrastructure.Caching;

public sealed class MemoryCacheManager : ICacheManager, Application.Abstractions.Caching.ICacheManager
{
    private readonly ObjectCache _cache = MemoryCache.Default;

    /// <summary>
    /// دریافت یک آیتم از کش با استفاده از کلید;
    /// </summary>
    /// <typeparam name="T">نوع آیتم ذخیره‌شده در کش;</typeparam>
    /// <param name="key">کلید آیتم برای دریافت;</param>
    /// <returns>آیتم ذخیره‌شده یا مقدار پیش‌فرض در صورت عدم وجود;</returns>
    /// <exception cref="ArgumentNullException">در صورت نال یا خالی بودن کلید پرتاب می‌شود;</exception>
    public T? Get<T>(string key)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        object? item = _cache[key];
        return item is T typedItem ? typedItem : default;
    }

    /// <summary>
    /// افزودن یا به‌روزرسانی یک آیتم در کش با زمان انقضای مشخص;
    /// </summary>
    /// <param name="key">کلید آیتم;</param>
    /// <param name="data">داده برای ذخیره در کش;</param>
    /// <param name="cacheTime">مدت زمان کش به دقیقه;</param>
    /// <exception cref="ArgumentNullException">در صورت نال یا خالی بودن کلید پرتاب می‌شود;</exception>
    /// <exception cref="ArgumentException">در صورت صفر یا منفی بودن زمان کش پرتاب می‌شود;</exception>
    public void Set(string key, object? data, int cacheTime)
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

    /// <summary>
    /// بررسی وجود یک آیتم در کش;
    /// </summary>
    /// <param name="key">کلید برای بررسی;</param>
    /// <returns>در صورت وجود آیتم true و در غیر این‌صورت false;</returns>
    /// <exception cref="ArgumentNullException">در صورت نال یا خالی بودن کلید پرتاب می‌شود;</exception>
    public bool IsSet(string key)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        return _cache.Contains(key);
    }

    /// <summary>
    /// حذف یک آیتم از کش با استفاده از کلید;
    /// </summary>
    /// <param name="key">کلید آیتم برای حذف;</param>
    /// <exception cref="ArgumentNullException">در صورت نال یا خالی بودن کلید پرتاب می‌شود;</exception>
    public void Remove(string key)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        _cache.Remove(key);
    }

    /// <summary>
    /// حذف آیتم‌های کش که با الگوی مشخص‌شده مطابقت دارند;
    /// </summary>
    /// <param name="pattern">الگوی regex برای تطبیق کلیدهای کش;</param>
    /// <exception cref="ArgumentNullException">در صورت نال یا خالی بودن الگو پرتاب می‌شود;</exception>
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

    /// <summary>
    /// پاک‌سازی تمام آیتم‌های موجود در کش;
    /// </summary>
    public void Clear()
    {
        var keys = _cache.Select(item => item.Key).ToList();
        foreach (string key in keys)
        {
            _cache.Remove(key);
        }
    }
}
