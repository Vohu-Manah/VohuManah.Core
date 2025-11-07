using SharedKernel;

namespace Domain.Library;

public static class SettingsErrors
{
    public static Error NotFound(string name) => Error.NotFound(
        "Settings.NotFound",
        $"The setting with the Name = '{name}' was not found");

    public static Error NotFound(int id) => Error.NotFound(
        "Settings.NotFound",
        $"The setting with the Id = '{id}' was not found");
}

