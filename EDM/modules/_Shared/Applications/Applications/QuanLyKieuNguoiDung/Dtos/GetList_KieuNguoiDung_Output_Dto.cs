using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyKieuNguoiDung.Dtos
{
    public class GetList_KieuNguoiDung_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbKieuNguoiDung> KieuNguoiDungs { get; set; }
    }
}