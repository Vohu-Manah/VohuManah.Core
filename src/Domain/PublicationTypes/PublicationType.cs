using VohuManah.SharedKernel;
using VohuManah.Domain.Publications;

namespace VohuManah.Domain.PublicationTypes;

public sealed class PublicationType : Entity
{
    public PublicationType()
    {
    }

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public ICollection<Publication> Publications { get; } = new List<Publication>();
}


