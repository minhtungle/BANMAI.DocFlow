
using EDM_DB;
using System;
using System.Web;

namespace Applications.QuanLyTaiLieu.Models
{
    public class tbTaiLieuExtend
    {
        public tbTaiLieu TaiLieu { get; set; }
        public Guid RowNumber { get; set; }
        public tbNhaCungCap NhaCungCap { get; set; }
        //public HttpPostedFileBase File { get; set; }
    }
}