using Applications.QuanLyLichHen.Dtos;
using Applications.QuanLyLichHen.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyLichHen.Interfaces
{
    public interface IQuanLyLichHenService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index();
        Task<List<tbLichHenExtend>> Get_LichHens
           (GetList_LichHen_Input_Dto input);
        Task<DisplayModal_CRUD_LichHen_Output_Dto> DisplayModal_CRUD_LichHen(
            DisplayModal_CRUD_LichHen_Input_Dto input);
        Task Create_LichHen(
            tbLichHenExtend lichHen);
        Task Update_LichHen(
            tbLichHenExtend lichHen);
        Task Delete_LichHens(
            List<Guid> idLichHens);
    }
}
