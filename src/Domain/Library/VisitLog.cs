using SharedKernel;

namespace Domain.Library;

public sealed class VisitLog : Entity
{
    public long Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public DateTime VisitedAt { get; set; }
}