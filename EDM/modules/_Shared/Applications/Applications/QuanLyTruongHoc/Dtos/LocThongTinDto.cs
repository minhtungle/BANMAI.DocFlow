using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyTruongHoc.Dtos
{
    public class LocThongTinDto
    {
        public List<Guid> IdTruongHocs { get; set; }
        public string TenTruongHoc { get; set; }
        public string Slug { get; set; }
        public string TenVietTat { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string GhiChu { get; set; }
    }
}