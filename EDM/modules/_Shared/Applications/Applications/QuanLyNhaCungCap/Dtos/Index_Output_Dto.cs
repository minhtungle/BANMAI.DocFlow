using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class Index_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbNhaCungCap> NhaCungCaps { get; set; }
    }
}