using SharedKernel;

namespace Domain.Library;

public sealed class Language : Entity
{
    public Language()
    {
        Books = new List<Book>();
        Manuscripts = new List<Manuscript>();
        Publications = new List<Publication>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; }
    public ICollection<Manuscript> Manuscripts { get; set; }
    public ICollection<Publication> Publications { get; set; }
}


