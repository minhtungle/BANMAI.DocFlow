using System;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class LocThongTinDto
    {
        public List<Guid> IdNhaCungCaps {  get; set; }
        public int Stt { get; set; }
        public string TenNhaCungCap { get; set; }
        public string Email {  get; set; }
        public string SoDienThoai {  get; set; }
        public string DiaChi {  get; set; }
        public string GhiChu {  get; set; }
    }
}