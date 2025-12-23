using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class GetList_TaiLieu_Input_Dto
    {
        public string Loai { get; set; } = "all";
        public LocThongTinDto LocThongTin { get; set; }
    }
}