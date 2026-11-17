using VohuManah.SharedKernel;
using VohuManah.Domain.Books;

namespace VohuManah.Domain.Publishers;

public sealed class Publisher : Entity
{
    public Publisher()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public int PlaceId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Book> Books { get; } = new List<Book>();
}


