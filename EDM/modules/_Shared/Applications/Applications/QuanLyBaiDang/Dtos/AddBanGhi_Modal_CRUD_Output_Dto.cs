using Applications.QuanLyBaiDang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class AddBanGhi_Modal_CRUD_Output_Dto
    {
        public string Loai { get; set; }
        public List<tbBaiDangExtend> BaiDangs { get; set; }
    }
}