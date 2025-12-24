using Applications.QuanLyNhaCungCap.Enums;
using Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyNhaCungCap.Models
{
    public class ValidationResultEx
    {
        public bool IsValid => InvalidFields.Count == 0;
        public List<FieldInvalidDto<NhaCungCapFieldEnum>> InvalidFields { get; set; }
            = new List<FieldInvalidDto<NhaCungCapFieldEnum>>();
    }
}