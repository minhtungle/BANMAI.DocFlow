using Applications.QuanLyNhaCungCap.Models;
using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class GetList_NhaCungCap_Output_Dto
    {
        public List<tbNhaCungCapExtend> NhaCungCaps { get; set; }
        public List<ThaoTac> ThaoTacs {  get; set; }
    }
}