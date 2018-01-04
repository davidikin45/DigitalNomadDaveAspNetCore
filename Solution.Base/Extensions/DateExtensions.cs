using Solution.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ToStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime ToEndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        //Json.NET returns all dates as Unspecified

        //UTC > LocalTime = Fine
        //LocalTime > UTC = Fine

        //Unspecified(UTC) > LocalTime
        //Unspecified(LocalTime) > UTC

        public static DateTime ToConfigLocalTime(this DateTime utcDT)
        {
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings("Timezone"));
            return TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ);
        }

        public static string ToConfigLocalTimeString(this DateTime utcDT)
        {
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings("Timezone"));
            return String.Format("{0} ({1})", TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToShortDateString(), ConfigurationManager.AppSettings("TimezoneAbbr"));
        }

        public static string ToConfigLocalTimeStringNoTimezone(this DateTime utcDT)
        {
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings("Timezone"));
            return String.Format("{0}", TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToShortDateString());
        }

        public static DateTime FromConfigLocalTimeToUTC(this DateTime localConfigDT)
        {
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings("Timezone"));
            return TimeZoneInfo.ConvertTimeToUtc(localConfigDT, istTZ);
        }
    }
}
