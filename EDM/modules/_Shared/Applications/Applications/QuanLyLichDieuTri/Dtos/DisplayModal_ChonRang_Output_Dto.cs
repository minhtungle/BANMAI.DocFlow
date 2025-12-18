using EDM_DB;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class DisplayModal_ChonRang_Output_Dto
    {
        public Guid IdTienTrinhDieuTri { get; set; }
        public SoDoHamRangDto SoDoHamRang_TreEm { get; set; }
        public SoDoHamRangDto SoDoHamRang_NguoiLon { get; set; }
        public List<int> SoRangDaChons { get; set; }
    }
}