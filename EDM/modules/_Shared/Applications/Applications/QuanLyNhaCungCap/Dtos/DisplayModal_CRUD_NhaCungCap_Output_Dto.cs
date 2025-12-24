using Applications.QuanLyNhaCungCap.Models;
using EDM_DB;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class DisplayModal_CRUD_NhaCungCap_Output_Dto
    {
        public tbNhaCungCapExtend NhaCungCap { get; set; }
        public List<tbNhaCungCapExtend> NhaCungCaps { get; set; }
        public List<tbTruongHoc> TruongHocs { get; set; }
        public string Loai { get; set; }
    }
}