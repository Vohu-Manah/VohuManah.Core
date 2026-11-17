using VohuManah.SharedKernel;
using System;

namespace VohuManah.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
