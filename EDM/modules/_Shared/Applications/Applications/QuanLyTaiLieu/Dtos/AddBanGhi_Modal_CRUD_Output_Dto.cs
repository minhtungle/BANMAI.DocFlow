using Applications.QuanLyTaiLieu.Models;
using EDM_DB;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyTaiLieu.Dtos {
    public class AddBanGhi_Modal_CRUD_Output_Dto {
        public List<tbTaiLieuExtend> TaiLieus { get; set; }
        public List<tbNhaCungCap> NhaCungCaps { get; set; }
        public Guid IdNhaCungCap { get; set; } // Nhà cung cấp đã chọn
        public string Loai { get; set; }
    }
}