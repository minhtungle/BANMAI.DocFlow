using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Applications.QuanLyTaiLieu.Models;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class DisplayModal_CRUD_TaiLieu_Output_Dto
    {
        public tbTaiLieuExtend TaiLieu { get; set; }
        public string Loai { get; set; }
    }
}