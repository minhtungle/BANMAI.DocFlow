using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum LoaiChiDinhEnum
    {
        [Display(Name = "Cephalometric")]
        [Description("Cephalometric")]
        _Cephalometric = 1,

        [Display(Name = "Panorama")]
        [Description("Panorama")]
        _Panorama,

        [Display(Name = "3D")]
        [Description("3D")]
        _3D,

        [Display(Name = "3D (5x5)")]
        [Description("3D (5x5)")]
        _3D5x5,

        [Display(Name = "Xét nghiệm huyết học")]
        [Description("Xét nghiệm huyết học")]
        _XetNghiemHuyetHoc,

        [Display(Name = "Xét nghiệm sinh hóa")]
        [Description("Xét nghiệm sinh hóa")]
        _XetNghiemSinhHoa,

        [Display(Name = "Ảnh (ext)")]
        [Description("Ảnh (ext)")]
        _AnhExt,

        [Display(Name = "Ảnh (int)")]
        [Description("Ảnh (int)")]
        _AnhInt,

        [Display(Name = "Cận chóp")]
        [Description("Cận chóp")]
        _CanChop,

        [Display(Name = "Khác")]
        [Description("Khác")]
        _Khac,
    }
}