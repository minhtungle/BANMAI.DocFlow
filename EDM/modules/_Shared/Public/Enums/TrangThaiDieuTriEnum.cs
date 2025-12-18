using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Public.Enums
{
    public enum TrangThaiDieuTriEnum
    {
        [Display(Name = "Chưa hoàn thành")]
        [Description("Chưa hoàn thành")]
        ChuaHoanThanh = 0,
        
        [Display(Name = "Đã hoàn thành")]
        [Description("Đã hoàn thành")]
        DaHoanThanh = 1,
    }
}