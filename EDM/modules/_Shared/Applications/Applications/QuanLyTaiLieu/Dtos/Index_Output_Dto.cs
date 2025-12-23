using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class Index_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public Guid IdNhaCungCap { get; set; }
        public List<tbNhaCungCap> NhaCungCaps { get; set; }
    }
}
