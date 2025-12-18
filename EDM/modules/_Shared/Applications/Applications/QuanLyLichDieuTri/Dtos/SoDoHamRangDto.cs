using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class SoDoHamRangDto
    {
        public List<default_tbRang> HamDuoi_BenPhai { get; set; }
        public List<default_tbRang> HamDuoi_BenTrai { get; set; }
        public List<default_tbRang> HamTren_BenPhai { get; set; }
        public List<default_tbRang> HamTren_BenTrai { get; set; }
    }
}