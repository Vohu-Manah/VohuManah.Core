namespace VohuManah.Application.Abstractions.SqlBackup;




public sealed record BackupProgress(
    bool IsRunning,
    int Percent,
    string Stage,
    string? FileName,
    string? Error,
    
    IReadOnlyList<string> Log);
