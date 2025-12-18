using System;

namespace Applications.QuanLyLichHen.Dtos
{
    public class DisplayModal_CRUD_LichHen_Input_Dto
    {
        public Guid IdLichHen { get; set; }
        public string Loai { get; set; } // create | update | view
    }
}