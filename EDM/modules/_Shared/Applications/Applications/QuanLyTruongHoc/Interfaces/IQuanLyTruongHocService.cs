using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Applications.QuanLyTruongHoc.Dtos;
using Applications.QuanLyTruongHoc.Models;
using EDM_DB;
using Public.Models;

namespace Applications.QuanLyTruongHoc.Interfaces
{
    public interface IQuanLyTruongHocService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index();
        Task<List<tbTruongHocExtend>> Get_TruongHocs(GetList_TruongHoc_Input_Dto input);
        Task<DisplayModal_CRUD_TruongHoc_Output_Dto> DisplayModal_CRUD_TruongHoc(
            DisplayModal_CRUD_TruongHoc_Input_Dto input);
        Task<bool> IsExisted_TruongHoc(tbTruongHoc truongHoc);
        Task Create_TruongHoc(tbTruongHocExtend truongHoc);
        Task Update_TruongHoc(tbTruongHocExtend truongHoc);
        Task Delete_TruongHocs(List<Guid> idTruongHocs);
    }
}
