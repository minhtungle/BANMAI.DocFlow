using Applications.QuanLyLichDieuTri.Dtos;
using Applications.QuanLyLichDieuTri.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyLichDieuTri.Interfaces
{
    public interface IQuanLyLichDieuTriService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index();
        Task<List<tbLichDieuTriExtend>> Get_LichDieuTris(GetList_LichDieuTri_Input_Dto input);
        Task<DisplayModal_CRUD_LichDieuTri_Output_Dto> DisplayModal_CRUD_LichDieuTri(DisplayModal_CRUD_LichDieuTri_Input_Dto input);
        Task<DisplayModal_ChonRang_Output_Dto> DisplayModal_ChonRang(DisplayModal_ChonRang_Input_Dto input);
        Task<tbThuThuat> ChonThuThuat(Guid idThuThuat);
        Task CapNhat_TongChiPhi_LichDieuTri(Guid idLichDieuTri);
        Task<bool> IsExisted_LichDieuTri(tbLichDieuTri lichDieuTri);
        Task Create_LichDieuTri(tbLichDieuTriExtend lichDieuTri);
        Task Update_LichDieuTri(tbLichDieuTriExtend lichDieuTri);
        Task Delete_LichDieuTris(List<Guid> idLichDieuTris);
    }
}