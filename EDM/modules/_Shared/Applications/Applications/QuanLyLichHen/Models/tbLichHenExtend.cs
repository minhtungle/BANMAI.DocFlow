using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLichHen.Models
{
    public class tbLichHenExtend
    {
        public tbLichHen LichHen { get; set; }
        public tbBenhNhan BenhNhan { get; set; }
        public tbBacSy BacSy { get; set; }
    }
}