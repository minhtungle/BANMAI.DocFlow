using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum TrangThaiBaoQuanEnum
    {
        [Display(Name = "Có thời hạn")]
        [Description("Có thời hạn")]
        CoThoiHan = 1,

        [Display(Name = "Vĩnh viễn")]
        [Description("Vĩnh viễn")]
        VinhVien = 2,
    }
}