using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LocThongTin_BaiDang = Applications.QuanLyBaiDang.Dtos.LocThongTinDto;

namespace Applications.QuanLyBaiDang.Interfaces
{
    public interface IQuanLyBaiDangService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_OutPut_Dto> Index_OutPut();
        Task<FormAddBaiDangDto> AddBanGhi_Modal_CRUD_Output(
            AddBanGhi_Modal_CRUD_Input_Dto input);
        Task<XemChiTiet_Output_Dto> XemChiTiet_BaiDang(Guid input);
        Task<List<tbBaiDangExtend>> GetBaiDangs
           (string loai = "all",
           List<Guid> idBaiDangs = null,
           LocThongTin_BaiDang locThongTin = null);

        Task<FreeImageUploadResponse> UploadToFreeImageHost(
            HttpPostedFileBase file);
        Task Create_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers);
        Task Update_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers);
        Task Delete_BaiDangs(
            List<Guid> idBaiDangs);
    }
}
