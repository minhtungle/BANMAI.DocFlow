using Applications.QuanLyBenhNhan.Dtos;
using Applications.QuanLyBenhNhan.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Applications.QuanLyBenhNhan.Interfaces
{
    public interface IQuanLyBenhNhanService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index();
        Task<List<tbBenhNhanExtend>> Get_BenhNhans
           (GetList_BenhNhan_Input_Dto input);
        Task<DisplayModal_CRUD_BenhNhan_Output_Dto> DisplayModal_CRUD_BenhNhan(
            DisplayModal_CRUD_BenhNhan_Input_Dto input);
        Task<bool> IsExisted_BenhNhan(tbBenhNhan benhNhan);
        Task Create_BenhNhan(
            tbBenhNhanExtend benhNhan);
        Task Update_BenhNhan(
            tbBenhNhanExtend benhNhan);
        Task Delete_BenhNhans(
            List<Guid> idBenhNhans);
        #region Xem chi tiết
        Task<XemChiTiet_BenhNhan_Output_Dto> XemChiTiet_BenhNhan(Guid idBenhNhan);
        Task<ShowTab_ThongTinCoBan_Output_Dto> ShowTab_ThongTinCoBan_BenhNhan(Guid idBenhNhan);
        Task<ShowTab_LichHen_Output_Dto> ShowTab_LichHen_BenhNhan(Guid idBenhNhan);
        Task<ShowTab_PhieuKham_Output_Dto> ShowTab_PhieuKham_BenhNhan(Guid idBenhNhan);
        //Task<ShowTab_DonThuoc_Output_Dto> ShowTab_DonThuoc_BenhNhan(Guid idBenhNhan);
        //Task<ShowTab_ThuVienAnh_Output_Dto> ShowTab_ThuVienAnh_BenhNhan(Guid idBenhNhan);
        //Task<ShowTab_XuongVatTu_Output_Dto> ShowTab_XuongVatTu_BenhNhan(Guid idBenhNhan);
        //Task<ShowTab_ThanhToan_Output_Dto> ShowTab_ThanhToan_BenhNhan(Guid idBenhNhan);
        //Task<ShowTab_BieuMau_Output_Dto> ShowTab_BieuMau_BenhNhan(Guid idBenhNhan);
        //Task<ShowTab_ChamSoc_Output_Dto> ShowTab_ChamSoc_BenhNhan(Guid idBenhNhan);
        //Task<ShowTab_LichSuThaoTac_Output_Dto> ShowTab_LichSuThaoTac_BenhNhan(Guid idBenhNhan);
        #endregion
    }
}