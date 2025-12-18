using System;

namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class DisplayModal_CRUD_LichDieuTri_Input_Dto
    {
        public Guid IdLichDieuTri { get; set; }
        public string Loai { get; set; } // create | update | view
    }
}