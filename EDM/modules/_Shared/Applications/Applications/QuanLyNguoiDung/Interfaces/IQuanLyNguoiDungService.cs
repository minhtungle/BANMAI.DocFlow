using Applications.QuanLyNguoiDung.Dtos;
using Applications.QuanLyNguoiDung.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyNguoiDung.Interfaces
{
    public interface IQuanLyNguoiDungService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index_OutPut();
        Task<List<tbNguoiDungExtend>> Get_NguoiDungs
           (string loai = "all",
           List<Guid> idNguoiDungs = null,
           LocThongTinDto locThongTin = null);
        Task<DisplayModal_CRUD_NguoiDung_Output_Dto> DisplayModal_CRUD_NguoiDung(
            DisplayModal_CRUD_NguoiDung_Input_Dto input);
        Task<bool> CapNhatNguoiDungHoatDong(Guid idNguoiDung);
        Task<bool> IsExisted_NguoiDung(tbNguoiDung nguoiDung);
        Task Create_NguoiDung(
            tbNguoiDungExtend nguoiDung);
        Task Update_NguoiDung(
            tbNguoiDungExtend nguoiDung);
        Task Update_MatKhau(
            tbNguoiDungExtend nguoiDung);
        Task Delete_NguoiDungs(
            List<Guid> idNguoiDungs);
    }
}