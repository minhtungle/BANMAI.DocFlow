using Applications.QuanLyBenhNhan.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class XemChiTiet_BenhNhan_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbBenhNhanExtend> BenhNhans { get; set; }
        public tbBenhNhanExtend BenhNhan { get; set; }
    }
}