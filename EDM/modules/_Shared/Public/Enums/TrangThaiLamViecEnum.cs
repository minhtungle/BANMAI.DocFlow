using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Public.Enums
{
    public enum TrangThaiLamViecEnum
    {
        [Display(Name = "Đã nghỉ việc")]
        [Description("Đã nghỉ việc")]
        DaNghiViec = 0,

        [Display(Name = "Đang làm việc")]
        [Description("Đang làm viêc")]
        DangLamViec = 1,

        [Display(Name = "Tạm nghỉ")]
        [Description("Tạm nghỉ")]
        TamNghi = 2,
    }
}