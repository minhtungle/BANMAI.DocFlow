using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Public.Enums
{
    public enum TrangThaiDuLieuEnum
    {
        [Display(Name = "Đang sử dụng")]
        [Description("Đang sử dụng")]
        DangSuDung = 1,

        [Display(Name = "Xóa bỏ hoàn toàn")]
        [Description("Xóa bỏ hoàn toàn và không khôi phục")]
        XoaBo = 0,

        [Display(Name = "Xóa bỏ tạm thời")]
        [Description("Xóa bỏ do xóa phần tử cha và có thể khôi phục")]
        XoaBoTamThoi = -1,

        [Display(Name = "Chờ phê duyệt")]
        [Description("Chờ quản lý phê duyệt dữ liệu")]
        ChoPheDuyet = -2,
    }
}