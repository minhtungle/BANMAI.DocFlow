using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class LocThongTinDto
    {
        public List<Guid> IdTaiLieus { get; set; }
        public string TenTaiLieu { get; set; }
        public string LoaiTep { get; set; }
        public string GhiChu { get; set; }
    }
}