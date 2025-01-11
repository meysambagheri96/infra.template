using System.Globalization;

namespace Campaign.Infrastructure.Utils.DateConverter;

public static class DateConverter
{
	public static string GetTimeOfDay(this DateTime dateTime) => dateTime.ToString("HH:mm");

	public static string GetPersianDateText(this DateTime dateTime)
	{
		var pc = new PersianCalendar();
		var year = pc.GetYear(dateTime);
		var month = pc.GetMonth(dateTime);
		var day = pc.GetDayOfMonth(dateTime);

		var dayOfWeek = dateTime.PersianDayOfWeek();
		var monthName = dateTime.GetMonthName();

		return $"{dayOfWeek} {day} {monthName} {year}";
	}
	
	public static DateTime GetPersianDate(this DateTime dateTime)
	{
		var pc = new PersianCalendar();
		var year = pc.GetYear(dateTime);
		var month= pc.GetMonth(dateTime);
		var day = pc.GetDayOfMonth(dateTime);

        return new DateTime(year, month,day);
    }

    public static string GetGregorianDateText(this DateTime dateTime) =>
        dateTime.ToString("yyyy MMMM dd", System.Globalization.CultureInfo.InvariantCulture);

	public static string GetMonthName(this DateTime date)
	{
		PersianCalendar pc = new PersianCalendar();
		int month = pc.GetMonth(date);

		switch (month)
		{
			case 1: return "فررودين";
			case 2: return "ارديبهشت";
			case 3: return "خرداد";
			case 4: return "تير";
			case 5: return "مرداد";
			case 6: return "شهريور";
			case 7: return "مهر";
			case 8: return "آبان";
			case 9: return "آذر";
			case 10: return "دي";
			case 11: return "بهمن";
			case 12: return "اسفند";
			default: return "";
		}
	}

	public static string PersianDayOfWeek(this DateTime date)
	{
		switch (date.DayOfWeek)
		{
			case DayOfWeek.Saturday:
				return "شنبه";
			case DayOfWeek.Sunday:
				return "یکشنبه";
			case DayOfWeek.Monday:
				return "دوشنبه";
			case DayOfWeek.Tuesday:
				return "سه شنبه";
			case DayOfWeek.Wednesday:
				return "چهارشنبه";
			case DayOfWeek.Thursday:
				return "پنجشنبه";
			case DayOfWeek.Friday:
				return "جمعه";
			default:
				throw new Exception();
		}
	}

	public static string GetGregorianDateWithArabicText(this DateTime dateTime)
	{
		var dayOfWeek = dateTime.ArabicDayOfWeek();

		return $"{dayOfWeek} {dateTime.Year}/{dateTime.Month}/{dateTime.Day}";
	}

	public static string ArabicDayOfWeek(this DateTime date)
	{
		switch (date.DayOfWeek)
		{
			case DayOfWeek.Saturday:
				return "السبت";
			case DayOfWeek.Sunday:
				return "الأحد";
			case DayOfWeek.Monday:
				return "الأثنين";
			case DayOfWeek.Tuesday:
				return "الثلاثاء";
			case DayOfWeek.Wednesday:
				return "الأربعاء";
			case DayOfWeek.Thursday:
				return "الخميس";
			case DayOfWeek.Friday:
				return "الجمعه";
			default:
				throw new Exception();
		}
	}

	public static string GetArabicMonthNameOfGregorian(this DateTime date)
	{
		int month = date.Month;

		switch (month)
		{
			case 1: return "یَنایر";
			case 2: return "فَبرایر";
			case 3: return "مارس";
			case 4: return "أبريل";
			case 5: return "مایو";
			case 6: return "يونيو";
			case 7: return "يوليو";
			case 8: return "أغسطس";
			case 9: return "سبتمبر";
			case 10: return "أكتوبر";
			case 11: return "نوفمبر";
			case 12: return "ديسمبر";
			default: return "";
		}
	}

	public static string GetArabicDateText(this DateTime dateTime)
	{
		var culture = System.Globalization.CultureInfo.GetCultureInfo("ar");
		var date = dateTime.ToString("d MMMM yyyy", culture);
		var dayOfWeek = culture.DateTimeFormat.GetDayName(dateTime.DayOfWeek);
		return $"{dayOfWeek} {date}";
	}

	public static string GetGregorianDateWhitArabicText(this DateTime dateTime)
	{
		var dayOfWeek = dateTime.ArabicDayOfWeek();
		var monthName = dateTime.GetArabicMonthNameOfGregorian();

		return $"{dayOfWeek} {dateTime.Day} {monthName} {dateTime.Year}";
	}

	public static string GetGregorianDateTimeWithArabicText(this DateTime dateTime)
	{
		var dayOfWeek = dateTime.ArabicDayOfWeek();
		var monthName = dateTime.GetArabicMonthNameOfGregorian();

		return $"{dayOfWeek} {dateTime.Day} {monthName} {dateTime.Year} {dateTime.Hour}:{dateTime.Minute}";
	}

	public static string GetPersianMonthName(this DateTime date)
	{
		PersianCalendar pc = new PersianCalendar();
		int month = pc.GetMonth(date);

		switch (month)
		{
			case 1: return "فررودين";
			case 2: return "اردیبهشت";
			case 3: return "خرداد";
			case 4: return "تیر";
			case 5: return "مرداد";
			case 6: return "شهریور";
			case 7: return "مهر";
			case 8: return "آبان";
			case 9: return "آذر";
			case 10: return "دی";
			case 11: return "بهمن";
			case 12: return "اسفند";
			default: return "";
		}
	}
}