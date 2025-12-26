using Applications.QuanLyNhaCungCap.Models;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Excel.Dtos
{
    public class ValidateImportData_Output_Dto
    {
        public List<tbNhaCungCapExtend> NhaCungCap_TongHop { get; set; } = new List<tbNhaCungCapExtend>(); // ✅ giữ vị trí
        public List<tbNhaCungCapExtend> NhaCungCap_HopLe { get; set; } = new List<tbNhaCungCapExtend>();
        public List<tbNhaCungCapExtend> NhaCungCap_KhongHopLe { get; set; } = new List<tbNhaCungCapExtend>();
    }
}