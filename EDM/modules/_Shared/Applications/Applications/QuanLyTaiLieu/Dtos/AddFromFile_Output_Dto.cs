using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Applications.QuanLyTaiLieu.Models;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class AddFromFile_Output_Dto
    {
        public List<tbTaiLieuExtend> TaiLieus { get; set; }
        public string Loai { get; set; } // create || update || view
    }
}