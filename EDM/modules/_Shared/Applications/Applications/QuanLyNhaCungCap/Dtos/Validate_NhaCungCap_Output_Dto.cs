using Applications.QuanLyNhaCungCap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class Validate_NhaCungCap_Output_Dto
    {
        public List<tbNhaCungCapExtend> NhaCungCap_HopLe { get; set; } = new List<tbNhaCungCapExtend>();
        public List<tbNhaCungCapExtend> NhaCungCap_KhongHopLe { get; set; } = new List<tbNhaCungCapExtend>();
    }
}