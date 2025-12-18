using Applications.QuanLyNguoiDung.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyNguoiDung.Dtos
{
    public class GetList_NguoiDung_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbNguoiDungExtend> NguoiDungs { get; set; }
        public Permission Permission { get; set; }
    }
}