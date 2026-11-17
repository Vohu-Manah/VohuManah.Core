using VohuManah.SharedKernel;
using VohuManah.Domain.Books;
using VohuManah.Domain.Manuscripts;
using VohuManah.Domain.Publications;

namespace VohuManah.Domain.Cities;

public sealed class City : Entity
{
    public City()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; } = new List<Book>();
    public ICollection<Manuscript> Manuscripts { get; } = new List<Manuscript>();
    public ICollection<Publication> Publications { get; } = new List<Publication>();
}


