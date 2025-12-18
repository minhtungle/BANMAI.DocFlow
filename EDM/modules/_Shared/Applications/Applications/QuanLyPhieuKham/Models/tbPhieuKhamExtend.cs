using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyPhieuKham.Models
{
    public class tbPhieuKhamExtend
    {
        public tbPhieuKham PhieuKham { get; set; }
        public tbBenhNhan BenhNhan { get; set; }
        public tbLichHen tbLichHen { get; set; }
        public tbBacSy BacSyKham { get; set; }
        public tbBacSy BacSyDieuTri { get; set; }
    }
}