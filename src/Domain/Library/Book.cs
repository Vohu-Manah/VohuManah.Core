using SharedKernel;

namespace Domain.Library;

public sealed class Book : Entity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Translator { get; set; } = string.Empty;
    public string Editor { get; set; } = string.Empty;
    public string Corrector { get; set; } = string.Empty;
    public int PublisherId { get; set; }
    public int PublishPlaceId { get; set; }
    public string PublishYear { get; set; } = string.Empty;
    public string PublishOrder { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string No { get; set; } = string.Empty;
    public int VolumeCount { get; set; }
    public int LanguageId { get; set; }
    public int SubjectId { get; set; }
    public string BookShelfRow { get; set; } = string.Empty;

    public Language? Language { get; set; }
    public Subject? Subject { get; set; }
    public City? City { get; set; }
    public Publisher? Publisher { get; set; }
}


