using Applications.QuanLyPhieuKham.Models;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyPhieuKham.Dtos
{
    public class GetList_PhieuKham_Output_Dto
    {
        public List<tbPhieuKhamExtend> LichHens { get; set; }
        public List<ThaoTac> ThaoTacs { get; set; }
    }
}