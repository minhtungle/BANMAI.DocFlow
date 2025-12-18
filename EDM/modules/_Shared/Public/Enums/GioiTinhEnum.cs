using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Public.Enums
{
    public enum GioiTinhEnum
    {
        [Display(Name = "Nam")]
        [Description("Nam")]
        Nam = 1,

        [Display(Name = "Nữ")]
        [Description("Nữ")]
        Nu = 0,
    }
}