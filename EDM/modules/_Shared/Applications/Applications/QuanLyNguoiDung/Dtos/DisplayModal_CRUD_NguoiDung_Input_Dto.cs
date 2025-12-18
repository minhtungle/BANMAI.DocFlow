using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyNguoiDung.Dtos
{
    public class DisplayModal_CRUD_NguoiDung_Input_Dto
    {
        public string Loai { get; set; }
        public Guid IdNguoiDung { get; set; }
    }
}