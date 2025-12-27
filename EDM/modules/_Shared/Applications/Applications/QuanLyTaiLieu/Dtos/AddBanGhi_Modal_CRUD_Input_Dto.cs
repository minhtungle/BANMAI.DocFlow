using System;
using System.Collections.Generic;
using System.Web;

namespace Applications.QuanLyTaiLieu.Dtos {
    public class AddBanGhi_Modal_CRUD_Input_Dto {
        public List<Guid> IdTaiLieus { get; set; }
        public Guid IdNhaCungCap { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public string Loai { get; set; } = "create";
    }
}