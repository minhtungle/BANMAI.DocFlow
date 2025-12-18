using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum LichHenLoaiThoiGianEnum
    {
        [Display(Name = "Đặt trước")]
        [Description("Đặt trước")]
        DatTruoc = 1,

        [Display(Name = "Phát sinh")]
        [Description("Phát sinh")]
        PhatSinh = 2,

        [Display(Name = "Dự kiến")]
        [Description("Dự kiến")]
        DuKien = 3,
    }
}