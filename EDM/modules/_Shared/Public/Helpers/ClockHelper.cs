using Public.Interfaces;
using System;

namespace Public.Helpers
{
    public class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
        public DateTimeOffset LocalNow => DateTimeOffset.Now;
    }

    public static class ClockHelper
    {
        public static IClock Current { get; set; } = new SystemClock();

        public static DateTimeOffset UtcNow => Current.UtcNow;
        public static DateTimeOffset LocalNow => Current.LocalNow;
    }

}