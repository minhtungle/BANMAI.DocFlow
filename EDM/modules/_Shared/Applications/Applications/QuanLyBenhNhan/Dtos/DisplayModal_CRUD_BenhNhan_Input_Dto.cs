using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class DisplayModal_CRUD_BenhNhan_Input_Dto
    {
        public Guid IdBenhNhan { get; set; }
        public string Loai { get; set; } // create | update | view
    }
}