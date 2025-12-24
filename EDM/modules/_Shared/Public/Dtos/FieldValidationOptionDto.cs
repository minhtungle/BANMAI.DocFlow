using Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Dtos
{
    public class FieldValidationOptionDto<T>
    {
        public T Field { get; set; }
        public ValidateRule Rules { get; set; } = ValidateRule.Required;
        public string DisplayName { get; set; }

        // Optional params for rules
        public int? MinLen { get; set; }
        public int? MaxLen { get; set; }
        public string Pattern { get; set; }          // Regex pattern
        public string PatternMessage { get; set; }   // Message khi regex fail
    }
}