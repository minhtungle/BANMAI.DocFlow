using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum LoaiRangEnum
    {
        [Display(Name = "Người lớn")]
        [Description("Người lớn")]
        NguoiLon = 1,

        [Display(Name = "Trẻ em")]
        [Description("Trẻ em")]
        TreEm = 2,
    }
}