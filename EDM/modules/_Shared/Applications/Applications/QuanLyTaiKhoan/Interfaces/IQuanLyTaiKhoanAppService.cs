using Applications.QuanLyTaiKhoan.Dtos;
using Applications.QuanLyTaiKhoan.Models;
using Applications.SocialApi.Dtos;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyTaiKhoan.Interfaces
{
    public interface IQuanLyTaiKhoanAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_OutPut_Dto> Index_OutPut();
        Task<List<tbTaiKhoanExtend>> GetTaiKhoans(
            string loai = "all",
            List<Guid> idTaiKhoans = null,
            LocThongTinDto locThongTin = null);
        Task<DisplayModel_CRUD_TaiKhoan_Output_Dto> DisplayModel_CRUD_TaiKhoan_Ouput(
            DisplayModel_CRUD_TaiKhoan_Input_Dto input);
        Task<tbNenTang> ChonNenTang(Guid input);
        Task<SocialUserWithPagesDto> GetUserAndPages(GetSocialInfo_Input_Dto input);
        Task<SocialPageDto> GetPageInfo(GetSocialInfo_Input_Dto input);
        Task<(bool, string, string)> Is_ValidToSave(tbTaiKhoan taiKhoan);
        Task Create_TaiKhoan(tbTaiKhoan taiKhoan);
        Task Update_TaiKhoan(tbTaiKhoan taiKhoan);
        Task Delete_TaiKhoan(List<Guid> idTaiKhoans);
    }
}
