using System.Globalization;

namespace Infrastructure.Helper;

internal sealed class PersianCalendarService : IPersianCalendar
{
    private readonly PersianCalendar _persianCalendar = new();

    public DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
    {
        return _persianCalendar.ToDateTime(year, month, day, hour, minute, second, millisecond);
    }

    public int GetYear(DateTime time)
    {
        return _persianCalendar.GetYear(time);
    }

    public int GetMonth(DateTime time)
    {
        return _persianCalendar.GetMonth(time);
    }

    public int GetDayOfMonth(DateTime time)
    {
        return _persianCalendar.GetDayOfMonth(time);
    }

    public DayOfWeek GetDayOfWeek(DateTime time)
    {
        return _persianCalendar.GetDayOfWeek(time);
    }

    public DateTime AddDays(DateTime time, int days)
    {
        return _persianCalendar.AddDays(time, days);
    }
}

