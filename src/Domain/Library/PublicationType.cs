using SharedKernel;

namespace Domain.Library;

public sealed class PublicationType : Entity
{
    public PublicationType()
    {
    }

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public ICollection<Publication> Publications { get; } = new List<Publication>();
}


