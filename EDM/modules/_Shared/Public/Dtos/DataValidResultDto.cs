using Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Dtos
{
    public class DataValidResultDto
    {
        public TrangThaiKiemTraDuLieuEnum TrangThaiKiemTraDuLieu { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }
}