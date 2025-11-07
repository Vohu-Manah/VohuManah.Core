using SharedKernel;

namespace Domain.Library;

public sealed class User : Entity
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}


