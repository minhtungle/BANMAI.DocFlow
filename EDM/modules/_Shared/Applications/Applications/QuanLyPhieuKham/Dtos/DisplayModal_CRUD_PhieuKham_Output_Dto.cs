using Applications.QuanLyPhieuKham.Models;
using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyPhieuKham.Dtos
{
    public class DisplayModal_CRUD_PhieuKham_Output_Dto
    {
        public tbPhieuKhamExtend PhieuKham { get; set; }
        public string Loai { get; set; }
        public List<tbBacSy> BacSys { get; set; }
    }
}