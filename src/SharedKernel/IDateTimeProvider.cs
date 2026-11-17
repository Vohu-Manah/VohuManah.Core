namespace VohuManah.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
