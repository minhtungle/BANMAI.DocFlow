using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLichDieuTri.Models
{
    public class tbTienTrinhDieuTriExtend
    {
        public tbTienTrinhDieuTri TienTrinhDieuTri { get; set; } = new tbTienTrinhDieuTri();
        public tbBacSy BacSyDieuTri { get; set; } = new tbBacSy();
        public tbBacSy PhuTa { get; set; } = new tbBacSy();
        public default_tbTinhTrangRang TinhTrangRang { get; set; } = new default_tbTinhTrangRang();
        public tbThuThuat ThuThuat { get; set; } = new tbThuThuat();
    }
}