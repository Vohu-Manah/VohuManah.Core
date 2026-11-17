using System.Diagnostics.CodeAnalysis;

namespace VohuManah.SharedKernel;

[SuppressMessage("Design", "S2094:Classes should not be empty",
    Justification = "Base class provides type hierarchy for domain entities")]
public abstract class Entity
{
}
