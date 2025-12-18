using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTruongHoc.Dtos
{
    public class GetList_TruongHoc_Input_Dto
    {
        public string Loai { get; set; } = "all";
        public LocThongTinDto LocThongTin { get; set; }
    }
}