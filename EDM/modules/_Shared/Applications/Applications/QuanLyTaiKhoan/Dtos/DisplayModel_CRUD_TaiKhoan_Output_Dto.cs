using Applications.QuanLyTaiKhoan.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTaiKhoan.Dtos
{
    public class DisplayModel_CRUD_TaiKhoan_Output_Dto
    {
        public tbTaiKhoanExtend TaiKhoan { get; set; } = new tbTaiKhoanExtend();
        public List<tbNenTang> NenTangs { get; set; } = new List<tbNenTang>();
        public string Loai { get; set; }
        public List<ThaoTac> ThaoTacs { get; set; } = new List<ThaoTac>();
    }
}