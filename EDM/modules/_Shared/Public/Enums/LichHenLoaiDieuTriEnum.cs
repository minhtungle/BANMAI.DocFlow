using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum LichHenLoaiDieuTriEnum
    {
        [Display(Name = "Khám mới")]
        [Description("Khám mới")]
        KhamMoi = 1,

        [Display(Name = "Tái khám")]
        [Description("Tái khám")]
        TaiKham = 2,

        [Display(Name = "Điều trị thủ thuật")]
        [Description("Điều trị thủ thuật")]
        DieuTriThuThuat = 3,
    }
}