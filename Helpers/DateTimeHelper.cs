public static class DateTimeHelper
{
    private static readonly TimeZoneInfo BrtZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    public static DateTime Now() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BrtZone);

    public static string NowAsString(string format = "yyyy-MM-dd HH:mm:ss") => Now().ToString(format);

    public static DateTime ConvertToBrasilia(DateTime utcDateTime) => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, BrtZone);

    public static DateTime ConvertToUtc(DateTime brasiliaDateTime) => TimeZoneInfo.ConvertTimeToUtc(brasiliaDateTime, BrtZone);
}
