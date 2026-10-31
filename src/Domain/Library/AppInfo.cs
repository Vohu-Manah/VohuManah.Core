using SharedKernel;

namespace Domain.Library;

public sealed class AppInfo : Entity
{
    public int Id { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string? Copyright { get; set; }
    public string? License { get; set; }
    public string? Website { get; set; }
    public string? SupportEmail { get; set; }
}


