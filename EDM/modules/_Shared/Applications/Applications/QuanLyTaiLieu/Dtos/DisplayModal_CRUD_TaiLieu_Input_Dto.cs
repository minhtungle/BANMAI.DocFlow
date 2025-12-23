using System;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class DisplayModal_CRUD_TaiLieu_Input_Dto
    {
        public Guid IdTaiLieu { get; set; }
        public string Loai { get; set; } // create | update | view
    }
}