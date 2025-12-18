using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyNguoiDung.Dtos
{
    public class Index_Output_Dto
    {
        public List<tbKieuNguoiDung> KieuNguoiDungs { get; set; }
        public List<tbCoCauToChuc> CoCauToChucs { get; set; }
        public List<default_tbChucVu> ChucVus { get; set; }
        public List<ThaoTac> ThaoTacs { get; set; }
    }
}