using Applications.QuanLyTaiLieu.Models;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyTaiLieu.Dtos
{
    public class GetList_TaiLieu_Output_Dto
    {
        public List<tbTaiLieuExtend> TaiLieus { get; set; }
        public List<ThaoTac> ThaoTacs { get; set; }
    }
}