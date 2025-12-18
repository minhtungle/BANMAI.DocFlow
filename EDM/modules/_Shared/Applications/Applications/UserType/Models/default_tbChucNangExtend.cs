using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.UserType.Models
{
    public class default_tbChucNangExtend
    {
        public default_tbChucNang ChucNang { get; set; }
        public List<default_tbChucNangExtend> ChucNangChilds { get; set; } = new List<default_tbChucNangExtend>();
    }
}