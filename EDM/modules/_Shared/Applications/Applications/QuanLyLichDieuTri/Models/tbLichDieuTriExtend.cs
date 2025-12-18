using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyLichDieuTri.Models
{
    public class tbLichDieuTriExtend
    {
        public tbLichDieuTri LichDieuTri { get; set; } = new tbLichDieuTri();
        public List<tbTienTrinhDieuTriExtend> TienTrinhDieuTris { get; set; } = new List<tbTienTrinhDieuTriExtend>();
        public List<tbLichDieuTri_AnhMoTa> AnhMoTas { get; set; } = new List<tbLichDieuTri_AnhMoTa>();
    }
}