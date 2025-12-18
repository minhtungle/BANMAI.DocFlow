using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class ShowTab_LichHen_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbLichHen> LichHens { get; set; }
    }
}