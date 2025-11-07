using SharedKernel;

namespace Domain.Library;

public sealed class Settings : Entity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}


