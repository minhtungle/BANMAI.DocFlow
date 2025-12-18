using System;

namespace Applications.QuanLyTruongHoc.Dtos
{
    public class DisplayModal_CRUD_TruongHoc_Input_Dto
    {
        public Guid IdTruongHoc { get; set; }
        public string Loai { get; set; } // create | update | view
    }
}