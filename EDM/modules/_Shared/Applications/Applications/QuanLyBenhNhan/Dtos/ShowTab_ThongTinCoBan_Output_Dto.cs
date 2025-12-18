using Applications.QuanLyBenhNhan.Models;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class ShowTab_ThongTinCoBan_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public tbBenhNhanExtend BenhNhan { get; set; }
    }
}