using System;
using System.Collections.Generic;
using System.Web;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class AddFromFile_Input_Dto
    {
        public Guid IdNhaCungCap { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
    }
}