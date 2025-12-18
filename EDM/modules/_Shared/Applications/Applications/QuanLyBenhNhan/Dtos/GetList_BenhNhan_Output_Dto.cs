using Applications.QuanLyBenhNhan.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class GetList_BenhNhan_Output_Dto
    {
        public List<tbBenhNhanExtend> BenhNhans { get; set; }
        public List<ThaoTac> ThaoTacs { get; set; }
    }
}