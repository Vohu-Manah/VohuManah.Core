namespace Application.Library.Settings.GetSystemInfo;

public sealed record SystemInfoResponse
{
    public string OperatingSystem { get; init; } = string.Empty;
    public string FrameworkVersion { get; init; } = string.Empty;
    public string MachineName { get; init; } = string.Empty;
    public string ProcessorCount { get; init; } = string.Empty;
    public string TotalMemory { get; init; } = string.Empty;
    public string AvailableMemory { get; init; } = string.Empty;
    public DateTime ServerTime { get; init; }
    public string TimeZone { get; init; } = string.Empty;
}


