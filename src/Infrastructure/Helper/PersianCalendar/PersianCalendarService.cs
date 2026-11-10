using System;

namespace VohuManah.Infrastructure.Helper.PersianCalendar;

internal sealed class PersianCalendarService : IPersianCalendar
{
    private const int PersianEpochJdn = 1948320; 
    private const int PersianCycleDays = 12053; 

    
    private static readonly bool[] LeapYears = CreateLeapTable();

    private static bool[] CreateLeapTable()
    {
        
        var table = new bool[33];
        int[] leaps = { 0, 4, 8, 12, 16, 21, 25, 29 };
        foreach (int i in leaps) table[i] = true;
        return table;
    }

    public DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
    {
        var g = JulianToGregorian(PersianToJulian(year, month, day));
        return new DateTime(g.Year, g.Month, g.Day, hour, minute, second, millisecond);
    }

    public int GetYear(DateTime time)
        => JulianToPersian(GregorianToJulian(time.Year, time.Month, time.Day)).Year;

    public int GetMonth(DateTime time)
        => JulianToPersian(GregorianToJulian(time.Year, time.Month, time.Day)).Month;

    public int GetDayOfMonth(DateTime time)
        => JulianToPersian(GregorianToJulian(time.Year, time.Month, time.Day)).Day;

    public DayOfWeek GetDayOfWeek(DateTime time)
        => time.DayOfWeek;

    public DateTime AddDays(DateTime time, int days)
        => time.AddDays(days);

    

    private static int GregorianToJulian(int year, int month, int day)
    {
        int a = (14 - month) / 12;
        int y = year + 4800 - a;
        int m = month + 12 * a - 3;
        return day + (153 * m + 2) / 5 + 365 * y + y / 4 - y / 100 + y / 400 - 32045;
    }

    private static (int Year, int Month, int Day) JulianToGregorian(int jd)
    {
        int a = jd + 32044;
        int b = (4 * a + 3) / 146097;
        int c = a - 146097 * b / 4;
        int d = (4 * c + 3) / 1461;
        int e = c - 1461 * d / 4;
        int m = (5 * e + 2) / 153;
        int day = e - (153 * m + 2) / 5 + 1;
        int month = m + 3 - 12 * (m / 10);
        int year = 100 * b + d - 4800 + m / 10;
        return (year, month, day);
    }

    private static int PersianToJulian(int year, int month, int day)
        => PersianEpochJdn + PersianDaysSinceEpoch(year, month, day);

    private static int PersianDaysSinceEpoch(int year, int month, int day)
    {
        int y = year - 1;
        int days = y * 365 + (y / 33) * 8 + ((y % 33 + 3) / 4);
        days += month <= 7 ? (month - 1) * 31 : 7 * 31 + (month - 7) * 30;
        return days + day - 1;
    }

    private static (int Year, int Month, int Day) JulianToPersian(int jd)
    {
        int days = jd - PersianEpochJdn;
        int cycles = days / PersianCycleDays;
        int rem = days % PersianCycleDays;
        int year = cycles * 33;

        int y = 0;
        while (true)
        {
            int daysInYear = LeapYears[y % 33] ? 366 : 365;
            if (rem < daysInYear) break;
            rem -= daysInYear;
            y++;
        }
        year += y + 1;

        int month, day;
        if (rem < 7 * 31)
        {
            month = rem / 31 + 1;
            day = rem % 31 + 1;
        }
        else
        {
            int rem2 = rem - 7 * 31;
            month = rem2 / 30 + 8;
            day = rem2 % 30 + 1;
        }
        return (year, month, day);
    }
}


