using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTaiKhoan.Models
{
    public class tbTaiKhoanExtend
    {
        public tbTaiKhoan TaiKhoan { get; set; } = new tbTaiKhoan();
        public tbNenTang NenTang { get; set; } = new tbNenTang();
    }
}