namespace Infrastructure.Helper;

public interface IPersianCalendar
{
    DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond);
    int GetYear(DateTime time);
    int GetMonth(DateTime time);
    int GetDayOfMonth(DateTime time);
    DayOfWeek GetDayOfWeek(DateTime time);
    DateTime AddDays(DateTime time, int days);
}

