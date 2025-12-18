using Applications.QuanLyLichHen.Models;
using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyLichHen.Dtos
{
    public class DisplayModal_CRUD_LichHen_Output_Dto
    {
        public tbLichHenExtend LichHen { get; set; }
        public string Loai { get; set; }
        public List<tbBacSy> BacSys { get; set; }
    }
}