using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyKieuNguoiDung.Dtos
{
    public class DisplayModal_CRUD_KieuNguoiDung_Input_Dto
    {
        public string Loai { get; set; }
        public Guid IdKieuNguoiDung { get; set; }
    }
}