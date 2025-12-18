using Microsoft.Extensions.Internal;
using Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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