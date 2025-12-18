using Applications.QuanLyNguoiDung.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyKieuNguoiDung.Dtos
{
    public class DisplayModal_CRUD_KieuNguoiDung_Output_Dto
    {
        public string Loai { get; set; }
        public tbKieuNguoiDung KieuNguoiDung { get; set; }
        public string HtmlChucNangs { get; set; }
    }
}