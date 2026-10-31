using SharedKernel;

namespace Domain.Library;

public sealed class Attachment : Entity
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string? Description { get; set; }
    public bool IsMain { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}


