namespace Application.Library.Backup.Shared;

public sealed record BackupInfoResponse(
    bool Exists,
    string FileName,
    string TakenAt,
    string TakenTime,
    long SizeBytes,
    bool IsRunning,
    int Percent,
    string Stage,
    string? Error,
    
    IReadOnlyList<string> Log);
