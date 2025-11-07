using SharedKernel;

namespace Domain.Library;

public sealed class Manuscript : Entity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int WritingPlaceId { get; set; }
    public string Year { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public string Size { get; set; } = string.Empty;
    public int GapId { get; set; }
    public string VersionNo { get; set; } = string.Empty;
    public int LanguageId { get; set; }
    public int SubjectId { get; set; }

    public Language? Language { get; set; }
    public Subject? Subject { get; set; }
    public City? City { get; set; }
    public Gap? Gap { get; set; }
}


