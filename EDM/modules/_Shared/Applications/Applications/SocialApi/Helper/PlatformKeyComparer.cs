using Applications.SocialApi.Enums;
using System;
using System.Collections.Generic;

namespace Applications.SocialApi.Helpers
{
    public class PlatformKeyComparer : IEqualityComparer<(string NenTang, LoaiApiEnum LoaiApi)>
    {
        public bool Equals((string, LoaiApiEnum) x, (string, LoaiApiEnum) y)
        {
            return string.Equals(x.Item1, y.Item1, StringComparison.OrdinalIgnoreCase) && x.Item2 == y.Item2;
        }

        public int GetHashCode((string, LoaiApiEnum) obj)
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Item1) ^ obj.Item2.GetHashCode();
        }
    }
}