using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Enums
{
    [Flags]
    public enum ValidateRule
    {
        None = 0,
        Required = 1,
        Format = 2,
        MinLength = 4,
        MaxLength = 8,
        Regex = 16
    }
}