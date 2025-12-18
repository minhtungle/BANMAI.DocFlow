using Applications.QuanLyLichHen.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLichHen.Dtos
{
    public class GetList_LichHen_Output_Dto
    {
        public List<tbLichHenExtend> LichHens { get; set; } 
        public List<ThaoTac> ThaoTacs { get; set; }
    }
}