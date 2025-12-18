using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Applications.QuanLyKieuNguoiDung.Dtos;
using Applications.QuanLyKieuNguoiDung.Models;

namespace Applications.QuanLyKieuNguoiDung.Interfaces
{
    public interface IQuanLyKieuNguoiDungService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index_OutPut();
        Task<List<tbKieuNguoiDung>> Get_KieuNguoiDungs
           (string loai = "all",
           List<Guid> idKieuNguoiDungs = null,
           LocThongTinDto locThongTin = null);
        Task<DisplayModal_CRUD_KieuNguoiDung_Output_Dto> DisplayModal_CRUD_KieuNguoiDung_Ouput(
            DisplayModal_CRUD_KieuNguoiDung_Input_Dto input);
        Task<bool> IsExisted_KieuNguoiDung(tbKieuNguoiDung kieuNguoiDung);
        Task Create_KieuNguoiDung(
            tbKieuNguoiDung kieuNguoiDung);
        Task Update_KieuNguoiDung(
            tbKieuNguoiDung kieuNguoiDung);
        Task Delete_KieuNguoiDungs(
            List<Guid> idKieuNguoiDungs,
            Guid idKieuNguoiDung_THAYTHE);
    }
}
