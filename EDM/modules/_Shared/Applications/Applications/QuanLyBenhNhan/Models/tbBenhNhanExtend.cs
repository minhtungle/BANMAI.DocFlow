using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Models
{
    public class tbBenhNhanExtend
    {
        public tbBenhNhan BenhNhan { get; set; }
        public List<tbBenhNhanNguoiThan> NguoiThans { get; set; } = new List<tbBenhNhanNguoiThan>();
    }
}