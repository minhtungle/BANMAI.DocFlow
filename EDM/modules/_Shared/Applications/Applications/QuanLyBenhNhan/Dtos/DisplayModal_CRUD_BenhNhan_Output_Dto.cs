using Applications.QuanLyBenhNhan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class DisplayModal_CRUD_BenhNhan_Output_Dto
    {
        public tbBenhNhanExtend BenhNhan { get; set; }
        public string Loai { get; set; }
    }
}