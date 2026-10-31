using SharedKernel;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Library;

public sealed class MenuItem : Entity
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } 

    
    public MenuItem? Parent { get; set; }

    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required by EF Core for navigation properties")]
    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
}

