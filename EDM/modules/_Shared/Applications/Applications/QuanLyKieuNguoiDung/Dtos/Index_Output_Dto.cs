using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyKieuNguoiDung.Dtos
{
    public class Index_Output_Dto
    {
        public List<tbKieuNguoiDung> KieuNguoiDungs { get; set; }
        public List<ThaoTac> ThaoTacs { get; set; }
    }
}