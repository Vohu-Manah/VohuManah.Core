using System;

namespace VohuManah.Infrastructure.Helper.PersianCalendar;

internal sealed class PersianCalendarService : IPersianCalendar
{
    private readonly global::System.Globalization.PersianCalendar _calendar = new();

    public DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
        => _calendar.ToDateTime(year, month, day, hour, minute, second, millisecond);

    public int GetYear(DateTime time)
        => _calendar.GetYear(time);

    public int GetMonth(DateTime time)
        => _calendar.GetMonth(time);

    public int GetDayOfMonth(DateTime time)
        => _calendar.GetDayOfMonth(time);

    public DayOfWeek GetDayOfWeek(DateTime time)
        => time.DayOfWeek;

    public DateTime AddDays(DateTime time, int days)
        => time.AddDays(days);
}
