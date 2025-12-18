using Applications.UserType.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.Home.Dtos
{
    public class Get_ChucNangs_Input_Dto
    {
        public List<default_tbChucNangExtend> ChucNangs { get; set; } = new List<default_tbChucNangExtend>();
    }
}