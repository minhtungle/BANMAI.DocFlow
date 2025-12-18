using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class AddBanGhi_Modal_CRUD_Input_Dto
    {
        public List<Guid> IdBaiDangs { get; set; }
        public string Loai { get; set; } = "create";
    }
}