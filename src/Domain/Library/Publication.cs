using SharedKernel;

namespace Domain.Library;

public sealed class Publication : Entity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public string Concessionaire { get; set; } = string.Empty;
    public string ResponsibleDirector { get; set; } = string.Empty;
    public string Editor { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string JournalNo { get; set; } = string.Empty;
    public string PublishDate { get; set; } = string.Empty;
    public int PublishPlaceId { get; set; }
    public string No { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int LanguageId { get; set; }
    public int SubjectId { get; set; }

    public Language? Language { get; set; }
    public Subject? Subject { get; set; }
    public City? City { get; set; }
    public PublicationType? PublicationType { get; set; }
}


