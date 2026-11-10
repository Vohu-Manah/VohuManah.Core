namespace Application.Library.Backup.Download;

public sealed record BackupDownloadFile
{
    public BackupDownloadFile(Stream content, string fileName, long contentLength)
    {
        Content = content;
        FileName = fileName;
        ContentLength = contentLength;
    }

    
    public Stream Content { get; }

    public string FileName { get; }

    
    public long ContentLength { get; }
}
