using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Models
{
    public class tbNhaCungCapExtend
    {
        public tbNhaCungCap NhaCungCap { get; set; }
        public List<tbTruongHoc> TruongHocs { get; set; } = new List<tbTruongHoc>();
        public List<tbTaiLieu> TaiLieus { get; set; } = new List<tbTaiLieu>();
        public int SoLuongTruongHoc { get; set; } = 0;
        public int SoLuongTaiLieu {  get; set; } = 0;
    }
}