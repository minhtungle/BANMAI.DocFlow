using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Dtos
{
    public class FieldInvalidDto<T>
    {
        public T Field { get; set; }
        public string Message { get; set; }
    }
}