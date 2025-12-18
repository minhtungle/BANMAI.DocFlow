using Applications.QuanLyPhieuKham.Dtos;
using Applications.QuanLyPhieuKham.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyPhieuKham.Interfaces
{
    public interface IQuanLyPhieuKhamService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index();
        Task<List<tbPhieuKhamExtend>> Get_PhieuKhams(GetList_PhieuKham_Input_Dto input);
        Task<DisplayModal_CRUD_PhieuKham_Output_Dto> DisplayModal_CRUD_PhieuKham(DisplayModal_CRUD_PhieuKham_Input_Dto input);
        Task<XemChiTiet_PhieuKham_Output_Dto> XemChiTiet_PhieuKham(Guid idPhieuKham);
        Task Create_PhieuKham(tbPhieuKhamExtend phieuKham);
        Task Update_PhieuKham(tbPhieuKhamExtend phieuKham);
        Task Delete_PhieuKhams(List<Guid> idPhieuKhams);
    }
}
