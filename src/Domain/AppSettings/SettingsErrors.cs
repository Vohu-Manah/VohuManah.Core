using VohuManah.SharedKernel;

namespace VohuManah.Domain.AppSettings;

public static class SettingsErrors
{
    public static Error NotFound(string name) => Error.NotFound(
        "Settings.NotFound",
        $"تنظیمات با نام '{name}' یافت نشد");

    public static Error NotFound(int id) => Error.NotFound(
        "Settings.NotFound",
        $"تنظیمات با شناسه '{id}' یافت نشد");
}

