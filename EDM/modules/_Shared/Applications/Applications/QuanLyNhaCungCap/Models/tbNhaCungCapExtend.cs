using EDM_DB;
using Public.Dtos;
using Public.Models;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Models {
    public class tbNhaCungCapExtend {
        public Guid RowNumber { get; set; } = Guid.NewGuid();
        public DataValidResultDto KiemTraDuLieu { get; set; } = new DataValidResultDto();
        public tbNhaCungCap NhaCungCap { get; set; }
        public tbNhaCungCap NhaCungCapCha { get; set; } = new tbNhaCungCap();
        public List<tbTruongHoc> TruongHocs { get; set; } = new List<tbTruongHoc>();
        public List<tbTaiLieu> TaiLieus { get; set; } = new List<tbTaiLieu>();
        public int SoLuongTruongHoc { get; set; } = 0;
        public int SoLuongTaiLieu { get; set; } = 0;
    }
}