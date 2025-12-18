using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyPhieuKham.Dtos
{
    public class XemChiTiet_PhieuKham_Output_Dto
    {
        public tbPhieuKham PhieuKham { get; set; } = new tbPhieuKham();
        public List<tbTienTrinhDieuTri> TienTrinhDieuTris { get; set; } = new List<tbTienTrinhDieuTri>();
    }
}