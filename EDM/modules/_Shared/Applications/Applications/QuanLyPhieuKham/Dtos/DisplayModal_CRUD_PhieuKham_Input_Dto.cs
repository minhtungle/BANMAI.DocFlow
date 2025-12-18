using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyPhieuKham.Dtos
{
    public class DisplayModal_CRUD_PhieuKham_Input_Dto
    {
        public Guid IdPhieuKham { get; set; }
        public string Loai { get; set; } // create | update | view
    }
}