using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTaiKhoan.Dtos
{
    public class DisplayModel_CRUD_TaiKhoan_Input_Dto
    {
        public Guid IdTaiKhoan { get; set; }
        public string Loai { get; set; }
    }
}