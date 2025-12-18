using Applications.QuanLyLichDieuTri.Models;
using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class ShowTab_PhieuKham_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public tbPhieuKham PhieuKham { get; set; }
        public List<tbLichDieuTriExtend> LichDieuTris { get; set; }
    }
}