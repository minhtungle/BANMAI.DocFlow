using Applications.QuanLyTaiLieu.Models;
using System.Collections.Generic;

namespace Applications.QuanLyTaiLieu.Dtos {
    public class AddBanGhi_Modal_CRUD_Output_Dto {
        public List<tbTaiLieuExtend> TaiLieus { get; set; }
        public string Loai { get; set; }
    }
}