using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class FormAddNhaCungCapDto
    {
        public AddBanGhi_Modal_CRUD_Output_Dto Data { get; set; }
        public List<tbNhaCungCap> NhaCungCaps { get; set; }
        public List<tbTruongHoc> TruongHocs { get; set; }
        public string LoaiView { get; set; }
    }
}