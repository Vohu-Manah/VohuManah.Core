namespace Application.Abstractions.Backup;

public sealed record BackupFileInfo(
    bool Exists,
    string FileName,
    string TakenAt,
    string TakenTime,
    long SizeBytes);
