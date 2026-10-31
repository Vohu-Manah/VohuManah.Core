using SharedKernel;

namespace Domain.Library;

public sealed class UserSettings : Entity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? ProfileImageAttachmentId { get; set; }
    public string? ThemePrimaryColor { get; set; }
    public string? ThemeSecondaryColor { get; set; }
    public string? ThemeAccentColor { get; set; }
    public string? ThemeMode { get; set; }

    
    public User? User { get; set; }
    public Attachment? ProfileImageAttachment { get; set; }
}
