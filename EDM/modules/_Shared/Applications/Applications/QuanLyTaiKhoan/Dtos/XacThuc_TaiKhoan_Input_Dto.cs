using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTaiKhoan.Dtos
{
    public class XacThuc_TaiKhoan_Input_Dto
    {
        public string AccessToken { get; set; }
        public Guid IdNenTang { get; set; }
        public int LoaiTaiKhoan { get; set; }
    }
}