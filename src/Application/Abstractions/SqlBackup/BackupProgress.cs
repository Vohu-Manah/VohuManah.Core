namespace Application.Abstractions.Backup;




public sealed record BackupProgress(
    bool IsRunning,
    int Percent,
    string Stage,
    string? FileName,
    string? Error,
    
    IReadOnlyList<string> Log);
