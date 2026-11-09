using SharedKernel;
using System;

namespace VohuManah.Infrastructure.Helper.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
