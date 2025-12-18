using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum ChucVuEnum
    {
        [Display(Name = "Giám đốc")]
        [Description("Giám đốc")]
        GiamDoc = 0,

        [Display(Name = "Bác sỹ")]
        [Description("Bác sỹ")]
        BacSy = 1,

        [Display(Name = "Lễ tân")]
        [Description("Lễ tân")]
        LeTan = 2,

        [Display(Name = "Phụ tá")]
        [Description("Phụ tá")]
        PhuTa = 3,
    }
}