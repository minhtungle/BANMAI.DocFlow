using Applications.QuanLyTaiKhoan.Models;
using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyTaiKhoan.Dtos
{
    public class GetList_TaiKhoan_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbTaiKhoanExtend> TaiKhoans { get; set; }
    }
}