using Applications.QuanLyTaiLieu.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class CreateByFile_Input_Dto
    {
        public Guid IdNhaCungCap { get; set; }
        public List<tbTaiLieuExtend> TaiLieus { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
    }
}