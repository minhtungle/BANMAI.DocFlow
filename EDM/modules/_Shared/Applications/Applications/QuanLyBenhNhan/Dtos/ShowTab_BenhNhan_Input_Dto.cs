using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class ShowTab_BenhNhan_Input_Dto
    {
        public string TabName { get; set; } = "thongtincoban";
        public Guid IdBenhNhan { get; set; }
    }
}