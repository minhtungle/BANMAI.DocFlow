using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class GetList_LichDieuTri_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbTienTrinhDieuTri> TienTrinhDieuTris { get; set; }
    }
}