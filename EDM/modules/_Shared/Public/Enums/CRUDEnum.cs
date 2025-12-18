using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum CRUDEnum
    {
        [Display(Name = "Thêm mới")]
        [Description("Thêm mới")]
        ThemMoi = 1,

        [Display(Name = "Cập nhật")]
        [Description("Cập nhật")]
        CapNhat = 2,

        [Display(Name = "Xóa bỏ")]
        [Description("Xóa bỏ")]
        XoaBo = 3,
    }
}