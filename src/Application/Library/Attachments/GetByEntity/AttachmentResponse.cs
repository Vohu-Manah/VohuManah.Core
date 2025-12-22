namespace Application.Library.Attachments.GetByEntity;

public sealed record AttachmentResponse
{
    public long Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string OriginalFileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}


