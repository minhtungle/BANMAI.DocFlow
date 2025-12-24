using Applications.QuanLyNhaCungCap.Models;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class AddBanGhi_Modal_CRUD_Output_Dto
    {
        public string Loai { get; set; }
        public List<tbNhaCungCapExtend> NhaCungCaps { get; set; }
    }
}