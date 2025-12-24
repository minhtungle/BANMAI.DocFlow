using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyNhaCungCap.Interfaces
{
    public interface IQuanLyNhaCungCapService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index();
        Task<List<tbNhaCungCapExtend>> Get_NhaCungCaps
           (GetList_NhaCungCap_Input_Dto input);
        Task<FormAddNhaCungCapDto> AddBanGhi_Modal_CRUD_Output(
          AddBanGhi_Modal_CRUD_Input_Dto input);
        Task<DisplayModal_CRUD_NhaCungCap_Output_Dto> DisplayModal_CRUD_NhaCungCap(
            DisplayModal_CRUD_NhaCungCap_Input_Dto input);
        Task Create_NhaCungCap(
            List<tbNhaCungCapExtend> nhaCungCaps);
        Task Update_NhaCungCap(
            tbNhaCungCapExtend nhaCungCap);
        Task Delete_NhaCungCaps(
            List<Guid> idNhaCungCaps);
    }
}
