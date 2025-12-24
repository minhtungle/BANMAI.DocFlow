using System;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class AddBanGhi_Modal_CRUD_Input_Dto
    {
        public List<Guid> IdNhaCungCaps { get; set; }
        public string Loai { get; set; } = "create";
    }
}