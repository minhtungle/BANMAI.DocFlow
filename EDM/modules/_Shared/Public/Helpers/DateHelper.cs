using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Helpers
{
    public static class DateHelper
    {
        public static (DateTime? Start, DateTime? End) ParseThangNam(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (null, null);

            var parts = input.Split('/');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int month) &&
                int.TryParse(parts[1], out int year))
            {
                try
                {
                    var start = new DateTime(year, month, 1);
                    var end = start.AddMonths(1).AddDays(-1);
                    return (start, end);
                }
                catch
                {
                    return (null, null);
                }
            }
            else if (parts.Length == 1 && int.TryParse(parts[0], out int onlyYear))
            {
                try
                {
                    var start = new DateTime(onlyYear, 1, 1);
                    var end = new DateTime(onlyYear, 12, 31);
                    return (start, end);
                }
                catch
                {
                    return (null, null);
                }
            }

            return (null, null);
        }
        public static string DateToString(DateTime? date, string format = "dd/MM/yyyy")
        {
            return date == null ? "" : date.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string DateToString(DateTimeOffset? date, string format = "dd/MM/yyyy")
        {
            return date == null ? "" : date.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string TimeToString(TimeSpan? time, string format = "hh:mm")
        {
            return time == null ? "" : $"{time.Value.Hours:D2}:{time.Value.Minutes:D2}";
        }
    }

}