using SharedKernel;

namespace Domain.Library;

public static class SettingsErrors
{
    public static Error NotFound(string name) => Error.NotFound(
        "Settings.NotFound",
        $"تنظیمات با نام '{name}' یافت نشد");

    public static Error NotFound(int id) => Error.NotFound(
        "Settings.NotFound",
        $"تنظیمات با شناسه '{id}' یافت نشد");
}

