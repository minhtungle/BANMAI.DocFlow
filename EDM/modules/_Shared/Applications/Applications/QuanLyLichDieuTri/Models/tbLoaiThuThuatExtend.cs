using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLichDieuTri.Models
{
    public class tbLoaiThuThuatExtend
    {
        public tbLoaiThuThuat LoaiThuThuat { get; set; }
        public List<tbThuThuat> ThuThuats { get; set; }
    }
}