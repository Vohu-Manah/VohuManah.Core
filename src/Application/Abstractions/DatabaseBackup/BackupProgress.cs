namespace VohuManah.Application.Abstractions.DatabaseBackup;




public sealed record BackupProgress(
    bool IsRunning,
    int Percent,
    string Stage,
    string? FileName,
    string? Error,
    
    IReadOnlyList<string> Log);
