using Application.Abstractions.Messaging;
using SharedKernel;
using System.Runtime.InteropServices;

namespace Application.Library.Settings.GetSystemInfo;

internal sealed class GetSystemInfoQueryHandler : IQueryHandler<GetSystemInfoQuery, SystemInfoResponse>
{
    public Task<Result<SystemInfoResponse>> Handle(GetSystemInfoQuery query, CancellationToken cancellationToken)
    {
        var osDescription = RuntimeInformation.OSDescription;
        var frameworkDescription = RuntimeInformation.FrameworkDescription;
        var machineName = Environment.MachineName;
        var processorCount = Environment.ProcessorCount.ToString();

        var totalMemory = GC.GetTotalMemory(false);
        var totalMemoryMB = (totalMemory / 1024.0 / 1024.0).ToString("F2");

        var workingSet = Environment.WorkingSet;
        var workingSetMB = (workingSet / 1024.0 / 1024.0).ToString("F2");

        var serverTime = DateTime.Now;
        var timeZone = TimeZoneInfo.Local.DisplayName;

        var response = new SystemInfoResponse
        {
            OperatingSystem = osDescription,
            FrameworkVersion = frameworkDescription,
            MachineName = machineName,
            ProcessorCount = processorCount,
            TotalMemory = $"{totalMemoryMB} MB",
            AvailableMemory = $"{workingSetMB} MB",
            ServerTime = serverTime,
            TimeZone = timeZone,
        };

        return Task.FromResult(Result.Success(response));
    }
}


