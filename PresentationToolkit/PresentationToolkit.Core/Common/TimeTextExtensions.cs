using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationToolkit.Core.Common
{
    /// <summary>
    /// Date/time related extension methods.
    /// </summary>
    public static class TimeTextExtensions
    {
        private const int Second = 1;
        private const int Minute = 60 * Second;
        private const int Hour = 60 * Minute;
        private const int Day = 24 * Hour;
        private const int Month = 30 * Day;

        /// <summary>
        /// Converts the specified time into words relative to now.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> to convert.</param>
        /// <returns>The relative time in words for the specified <see cref="DateTime"/></returns>
        public static string RelativeText(this DateTime dateTime)
        {
            DateTime dateTimeValue = dateTime.Kind == DateTimeKind.Local
                ? dateTime.ToUniversalTime()
                : dateTime;

            var timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - dateTimeValue.Ticks);
            double delta = Math.Abs(timeSpan.TotalSeconds);

            if (delta < 1 * Minute)
            {
                return timeSpan.Seconds == 1
                    ? "one second ago"
                    : timeSpan.Seconds + " seconds ago";
            }

            if (delta < 2 * Minute)
            {
                return "a minute ago";
            }

            if (delta < 45 * Minute)
            {
                return timeSpan.Minutes + " minutes ago";
            }

            if (delta < 90 * Minute)
            {
                return "an hour ago";
            }

            if (delta < 24 * Hour)
            {
                return timeSpan.Hours + " hours ago";
            }

            if (delta < 48 * Hour)
            {
                return "yesterday";
            }

            if (delta < 30 * Day)
            {
                return timeSpan.Days + " days ago";
            }

            if (delta < 12 * Month)
            {
                int months = (int)(Math.Floor((double)timeSpan.Days / 30));
                return months <= 1
                    ? "one month ago"
                    : months + " months ago";
            }

            int years = (int)(Math.Floor((double)timeSpan.Days / 365));
            return years <= 1
                ? "one year ago"
                : years + " years ago";
        }

        /// <summary>
        /// Converts the specified duration into words.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert.</param>
        /// <returns>The duration in words.</returns>
        public static string DurationText(this TimeSpan timeSpan)
        {
            // Day part
            string dayPart = null;
            if (timeSpan.Days > 0)
            {
                dayPart = string.Format(timeSpan.Days == 1
                    ? "{0} day "
                    : "{0} days ",
                    timeSpan.Days);
            }

            TimeSpan offset = timeSpan.Subtract(TimeSpan.FromDays(timeSpan.Days));
            if (!string.IsNullOrEmpty(dayPart))
            {
                if (offset.TotalSeconds > 0)
                {
                    return "Over " + dayPart;
                }

                return dayPart;
            }

            // Hours
            string hourPart = null;
            offset = offset.Subtract(TimeSpan.FromHours(offset.Hours));
            if (timeSpan.Hours > 0)
            {
                hourPart = string.Format(timeSpan.Hours == 1
                    ? "{0} hour "
                    : "{0} hours ",
                    timeSpan.Hours);
            }

            if (!string.IsNullOrEmpty(hourPart))
            {
                if (offset.TotalSeconds > 0)
                {
                    return "Over " + hourPart;
                }

                return hourPart;
            }

            // Minutes
            string minutePart = null;
            offset = offset.Subtract(TimeSpan.FromMinutes(offset.Minutes));
            if (timeSpan.Minutes > 0)
            {
                minutePart = string.Format(timeSpan.Minutes == 1
                    ? "{0} minute "
                    : "{0} minutes ",
                    timeSpan.Minutes);
            }
            if (!string.IsNullOrEmpty(minutePart))
            {
                if (offset.Seconds > 0)
                {
                    return "Over " + minutePart;
                }

                return minutePart;
            }

            // Seconds
            return string.Format(timeSpan.Seconds == 1
                ? "{0} second"
                : "{0} seconds",
                timeSpan.Seconds);
        }

        /// <summary>
        /// Converts the specified duration into words.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert.</param>
        /// <returns>The duration in words.</returns>
        public static string DurationFullText(this TimeSpan timeSpan)
        {
            var content = new List<string>();
            // Day part
            if (timeSpan.Days > 0)
            {
                content.Add(string.Format(timeSpan.Days == 1
                    ? "{0} day "
                    : "{0} days ",
                    timeSpan.Days));
            }

            // Hours
            if (timeSpan.Hours > 0)
            {
                content.Add(string.Format(timeSpan.Hours == 1
                    ? "{0} hour "
                    : "{0} hours ",
                    timeSpan.Hours));
            }

            // Minutes
            if (timeSpan.Minutes > 0)
            {
                content.Add(string.Format(timeSpan.Minutes == 1
                    ? "{0} minute "
                    : "{0} minutes ",
                    timeSpan.Minutes));
            }

            // Seconds
            if (timeSpan.Seconds > 0)
            {
                content.Add(string.Format(timeSpan.Seconds == 1
                    ? "{0} second"
                    : "{0} seconds",
                    timeSpan.Seconds));
            }

            if (content.Count == 1)
            {
                return content.First();
            }

            var builder = new StringBuilder();
            for (int i = 0; i < content.Count - 1; i++)
            {
                builder.Append(content[i]);
            }
            if (content.Count > 1)
            {
                if (builder.Length > 1)
                {
                    builder.Append(" and ");
                }
                builder.Append(content.Last());
            }

            return builder.ToString();
        }
    }
}
