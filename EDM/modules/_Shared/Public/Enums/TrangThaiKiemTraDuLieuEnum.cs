using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Public.Enums
{
    public enum TrangThaiKiemTraDuLieuEnum
    {
        [Display(Name = "Hợp lệ")]
        [Description("Hợp lệ")]
        HopLe = 1,

        [Display(Name = "Không hợp lệ")]
        [Description("Không hợp lệ")]
        KhongHopLe = 0,
    }
}