using SharedKernel;

namespace Domain.Library;

public sealed class Gap : Entity
{
    public Gap()
    {
        Manuscripts = new List<Manuscript>();
    }

    public int Id { get; set; }
    public bool State { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;

    public ICollection<Manuscript> Manuscripts { get; set; }
}


