using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Applications.QuanLyTruongHoc.Models;

namespace Applications.QuanLyTruongHoc.Dtos
{
    public class DisplayModal_CRUD_TruongHoc_Output_Dto
    {
        public tbTruongHocExtend TruongHoc { get; set; }
        public string Loai { get; set; }
    }
}