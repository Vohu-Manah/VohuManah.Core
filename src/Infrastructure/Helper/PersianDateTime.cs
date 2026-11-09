using System.Globalization;

namespace Infrastructure.Helper;

#pragma warning disable CA1846

public class PersianDateTime
{
    private readonly IPersianCalendar _persianCalendar;

    public PersianDateTime(IPersianCalendar persianCalendar)
    {
        _persianCalendar = persianCalendar;
    }

    public string PerianDate()
    {
        DateTime now = DateTime.Now;
        string year = _persianCalendar.GetYear(now).ToString(CultureInfo.InvariantCulture);
        string month = _persianCalendar.GetMonth(now).ToString(CultureInfo.InvariantCulture);
        string day = _persianCalendar.GetDayOfMonth(now).ToString(CultureInfo.InvariantCulture);

        if (month.Trim().Length == 1)
        {
            month = "0" + month.Trim();
        }

        if (day.Trim().Length == 1)
        {
            day = "0" + day.Trim();
        }

        return $"{year}/{month}/{day}";
    }

    public string ConvertPersianDateToGregorianDate(string persianDate)
    {
        try
        {
            return _persianCalendar.ToDateTime(
                int.Parse(persianDate.Trim().Substring(0, 4), CultureInfo.InvariantCulture),
                int.Parse(persianDate.Trim().Substring(5, 2), CultureInfo.InvariantCulture),
                int.Parse(persianDate.Trim().Substring(8, 2), CultureInfo.InvariantCulture),
                0, 0, 0, 0).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        catch
        {
            return string.Empty;
        }
    }

    public string ConvertGregorianDateToPersianDate(string gregorianDate)
    {
        try
        {
            var time = Convert.ToDateTime(DateTime.ParseExact(gregorianDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            string year = _persianCalendar.GetYear(time).ToString(CultureInfo.InvariantCulture);
            string month = _persianCalendar.GetMonth(time).ToString(CultureInfo.InvariantCulture);
            string day = _persianCalendar.GetDayOfMonth(time).ToString(CultureInfo.InvariantCulture);

            if (month.Trim().Length == 1)
            {
                month = "0" + month.Trim();
            }

            if (day.Trim().Length == 1)
            {
                day = "0" + day.Trim();
            }

            return $"{year}/{month}/{day}";
        }
        catch
        {
            return string.Empty;
        }
    }

    public string GetPersianMonthName(string persianDate)
    {
        int month = int.Parse(persianDate.Substring(5, 2), CultureInfo.InvariantCulture);
        return Months[month];
    }

    public string GetPersianDayName()
    {
        DateTime now = DateTime.Now;
        byte dayOfWeek = (byte)_persianCalendar.GetDayOfWeek(now);
        return Days[dayOfWeek];
    }

    public string GetPersianDay(string persianDate)
    {
        string result = string.Empty;
        switch (int.Parse(persianDate.Substring(8, 2), CultureInfo.InvariantCulture))
        {
            case 1:
                result = " اول ";
                break;
            case 2:
                result = " دوم ";
                break;
            case 3:
                result = " سوم ";
                break;
            case 4:
                result = " چهارم ";
                break;
            case 5:
                result = " پنجم ";
                break;
            case 6:
                result = " ششم ";
                break;
            case 7:
                result = " هفتم ";
                break;
            case 8:
                result = " هشتم ";
                break;
            case 9:
                result = " نهم ";
                break;
            case 10:
                result = " دهم ";
                break;
            case 11:
                result = " يازدهم ";
                break;
            case 12:
                result = " دوازدهم ";
                break;
            case 13:
                result = " سيزدهم ";
                break;
            case 14:
                result = " چهاردهم ";
                break;
            case 15:
                result = " پانزدهم ";
                break;
            case 16:
                result = " شانزدهم ";
                break;
            case 17:
                result = " هفدهم ";
                break;
            case 18:
                result = " هيجدهم ";
                break;
            case 19:
                result = " نوزدهم ";
                break;
            case 20:
                result = " بيستم ";
                break;
            case 21:
                result = " بيست و يکم ";
                break;
            case 22:
                result = " بيست و دوم ";
                break;
            case 23:
                result = " بيست و سوم ";
                break;
            case 24:
                result = " بيست و چهارم ";
                break;
            case 25:
                result = " بيست و پنجم ";
                break;
            case 26:
                result = " بيست و ششم ";
                break;
            case 27:
                result = " بيست هفتم ";
                break;
            case 28:
                result = " بيست و هشتم ";
                break;
            case 29:
                result = " بيست و نهم ";
                break;
            case 30:
                result = " سی ام ";
                break;
            case 31:
                result = " سی و يکم ";
                break;
        }
        return result;
    }

    public string AddMonth(string date, short count)
    {
        int num = int.Parse(date.Substring(5, 2), CultureInfo.InvariantCulture);
        int num2 = ((num + (int)count) % 12 == 0) ? 12 : ((num + (int)count) % 12);
        int num3 = (int)Convert.ToInt16(date.Substring(0, 4), CultureInfo.InvariantCulture);
        decimal d = Convert.ToDecimal((num + (int)count - 1) / 12);
        int num4 = num3 + Convert.ToInt32(Math.Floor(d));
        int num5 = (int)Convert.ToByte(date.Substring(8, 2), CultureInfo.InvariantCulture);
        int num6 = num5;
        bool flag = num5 == 31 && num2 > 6;
        if (flag)
        {
            num2++;
            bool flag2 = num2 > 12;
            if (flag2)
            {
                num2 = 1;
                num4++;
            }
            num6 = 1;
        }
        return string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}",
            num4.ToString(CultureInfo.InvariantCulture),
            (num2.ToString(CultureInfo.InvariantCulture).Length == 1) ? "0" + num2.ToString(CultureInfo.InvariantCulture) : num2.ToString(CultureInfo.InvariantCulture),
            (num6.ToString(CultureInfo.InvariantCulture).Length == 1) ? "0" + num6.ToString(CultureInfo.InvariantCulture) : num6.ToString(CultureInfo.InvariantCulture));
    }

    public string AddDay(string date, int dayCount)
    {
        return ConvertGregorianDateToPersianDate(_persianCalendar.AddDays(DateTime.Parse(ConvertPersianDateToGregorianDate(date), CultureInfo.InvariantCulture).Date, dayCount).ToString(CultureInfo.InvariantCulture));
    }

    public string GetEndDayOfyear(string year)
    {
        string dates = year + "/12/01";
        return year + "/12/" + GetEndDayForMonth(dates).ToString(CultureInfo.InvariantCulture);
    }

    public bool IsLeapYear(int year)
    {
        int num = year % 33;
        return num == 1 || num == 5 || num == 9 || num == 13 || num == 18 || num == 22 || num == 26 || num == 30;
    }

    public int GetEndDayForMonth(string dates)
    {
        int year = int.Parse(dates.Substring(0, 4), CultureInfo.InvariantCulture);
        byte month = Convert.ToByte(dates.Substring(5, 2), CultureInfo.InvariantCulture);

        return month switch
        {
            <= 6 => 31,
            >= 7 and <= 11 => 30,
            12 => IsLeapYear(year) ? 30 : 29,
            _ => 31
        };
    }

    private static readonly string[] Days =
    [
        "يكشنبه",
        "دوشنبه",
        " سه شنبه ",
        " چهارشنبه ",
        " پنجشنبه ",
        " جمعه ",
        " شنبه "
    ];

    private static readonly string[] Months =
    [
        "",
        " فروردين",
        " ارديبهشت",
        " خرداد",
        " تير",
        " مرداد",
        " شهريور",
        " مهر",
        " آبان",
        " آذر",
        " دي",
        " بهمن",
        " اسفند"
    ];
}

