using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Interfaces
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
        DateTimeOffset LocalNow { get; }
    }

}