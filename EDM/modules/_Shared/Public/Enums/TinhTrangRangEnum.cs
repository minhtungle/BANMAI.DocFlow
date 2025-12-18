using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum TinhTrangRangEnum
    {
        [Display(Name = "SR")]
        [Description("Sâu răng")]
        _SauRang = 1,

        [Display(Name = "VT")]
        [Description("Viêm tủy")]
        _ViemTuy,

        [Display(Name = "MR")]
        [Description("Mất răng")]
        _MatRang,

        [Display(Name = "RNG")]
        [Description("Răng nứt gãy")]
        _RangNutGay,

        [Display(Name = "RMN")]
        [Description("Răng mọc ngầm")]
        _RangMocNgam,

        [Display(Name = "RKK")]
        [Description("Răng khấp khểnh")]
        _RangKhapKhenh,

        [Display(Name = "Imp")]
        [Description("Implant")]
        _Implant,

        [Display(Name = "VL")]
        [Description("Viêm lợi")]
        _ViemLoi,

        [Display(Name = "Khác")]
        [Description("Khác")]
        _Khac
    }
}