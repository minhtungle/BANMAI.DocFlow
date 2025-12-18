using System;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class DisplayModal_CRUD_NhaCungCap_Input_Dto
    {
        public Guid IdNhaCungCap { get; set; }
        public string Loai { get; set; } // create | update | view
    }
}