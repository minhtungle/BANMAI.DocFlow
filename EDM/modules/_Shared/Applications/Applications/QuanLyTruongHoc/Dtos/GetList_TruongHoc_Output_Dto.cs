using Applications.QuanLyTruongHoc.Models;
using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyTruongHoc.Dtos
{
    public class GetList_TruongHoc_Output_Dto
    {
        public List<tbTruongHocExtend> TruongHocs { get; set; }
        public List<ThaoTac> ThaoTacs { get; set; }
    }
}