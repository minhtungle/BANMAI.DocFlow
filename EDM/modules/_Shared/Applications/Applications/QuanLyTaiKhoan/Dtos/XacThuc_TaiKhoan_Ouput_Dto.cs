using Applications.QuanLyTaiKhoan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTaiKhoan.Dtos
{
    public class XacThuc_TaiKhoan_Ouput_Dto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public MetaApiResult Data { get; set; }
    }
}