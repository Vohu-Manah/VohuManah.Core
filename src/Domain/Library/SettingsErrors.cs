using SharedKernel;

namespace Domain.Library;

public static class SettingsErrors
{
    public static Error NotFound(string name) => Error.NotFound(
        "Settings.NotFound",
        $"تنظیم با نام '{name}' یافت نشد");

    public static Error NotFound(int id) => Error.NotFound(
        "Settings.NotFound",
        $"تنظیم با شناسه '{id}' یافت نشد");
}

