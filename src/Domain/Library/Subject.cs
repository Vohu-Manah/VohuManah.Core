using SharedKernel;

namespace Domain.Library;

public sealed class Subject : Entity
{
    public Subject()
    {
    }
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public ICollection<Book> Books { get; } = new List<Book>();
    public ICollection<Manuscript> Manuscripts { get; } = new List<Manuscript>();
    public ICollection<Publication> Publications { get; } = new List<Publication>();
}


