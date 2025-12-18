using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class DisplayModal_ChonRang_Input_Dto
    {
        public Guid IdTienTrinhDieuTri { get; set; }
        public List<int> SoRangDaChons { get; set; }
    }
}