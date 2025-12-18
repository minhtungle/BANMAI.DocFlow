using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum LichHenTrangThaiKhamEnum
    {
        [Display(Name = "Hủy hẹn")]
        [Description("Hủy hẹn")]
        HuyHen = 0,

        [Display(Name = "Đã xong")]
        [Description("Đã xong")]
        DaXong = 1,

        [Display(Name = "Hẹn lại")]
        [Description("Hẹn lại")]
        HenLai = 2,

        [Display(Name = "Chưa đến hẹn")]
        [Description("Chưa đến hẹn")]
        ChuaDenHen = 3,
    }
}