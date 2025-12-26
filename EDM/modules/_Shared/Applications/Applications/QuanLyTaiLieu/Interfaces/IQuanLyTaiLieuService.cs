using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Applications.QuanLyTaiLieu.Dtos;
using Applications.QuanLyTaiLieu.Models;
using Public.Models;
using EDM_DB;
using System.Web;

namespace Applications.QuanLyTaiLieu.Interfaces {
    public interface IQuanLyTaiLieuService {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index(Index_Input_Dto input);
        Task<List<tbTaiLieuExtend>> Get_TaiLieus(GetList_TaiLieu_Input_Dto input);
        Task<DisplayModal_CRUD_TaiLieu_Output_Dto> DisplayModal_CRUD_TaiLieu(DisplayModal_CRUD_TaiLieu_Input_Dto input);
        Task<bool> IsExisted_TaiLieu(tbTaiLieu taiLieu);
        Task Create_TaiLieu(List<tbTaiLieuExtend> taiLieus, HttpPostedFileBase[] files);
        Task Update_TaiLieu(tbTaiLieuExtend taiLieu);
        Task Delete_TaiLieus(List<Guid> idTaiLieus);
    }
}
