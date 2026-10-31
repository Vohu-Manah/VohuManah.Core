using SharedKernel;

namespace Domain.Library;

public sealed class Language : Entity
{
    public Language()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;

    public ICollection<Book> Books { get; } = new List<Book>();
    public ICollection<Manuscript> Manuscripts { get; } = new List<Manuscript>();
    public ICollection<Publication> Publications { get; } = new List<Publication>();
}


