using Applications.QuanLyLichDieuTri.Models;
using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class DisplayModal_CRUD_LichDieuTri_Output_Dto
    {
        public tbLichDieuTriExtend LichDieuTri { get; set; }
        public List<tbLoaiThuThuatExtend> LoaiThuThuats { get; set; }
        public List<tbBacSy> BacSys { get; set; }
        public List<default_tbTinhTrangRang> TinhTrangRangs { get; set; }
        public string Loai { get; set; }
    }
}