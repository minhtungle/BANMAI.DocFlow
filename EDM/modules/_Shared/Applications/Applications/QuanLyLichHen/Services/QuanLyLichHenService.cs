using Applications.QuanLyLichHen.Dtos;
using Applications.QuanLyLichHen.Interfaces;
using Applications.QuanLyLichHen.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Applications.QuanLyLichHen.Filters;
using System.Data.Entity;
using Public.Enums;

namespace Applications.QuanLyLichHen.Services
{
    public class QuanLyLichHenService : BaseService, IQuanLyLichHenService
    {
        private readonly IRepository<tbLichHen, Guid> _lichHenRepo;
        private readonly IRepository<tbBacSy, Guid> _bacSyRepo;
        public QuanLyLichHenService(
          IUserContext userContext,
          IUnitOfWork unitOfWork,

          IRepository<tbLichHen, Guid> lichHenRepo,
          IRepository<tbBacSy, Guid> bacSyRepo
          ) : base(userContext, unitOfWork)
        {
            _lichHenRepo = lichHenRepo;
            _bacSyRepo = bacSyRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_Output_Dto> Index()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyLichHen");
            //var lichHens = await Get_LichHens(input: new GetList_LichHen_Input_Dto());
            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }

        #region Lịch hẹn
        public async Task<List<tbLichHenExtend>> Get_LichHens(GetList_LichHen_Input_Dto input)
        {
            var query = _lichHenRepo.Query()
                .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin.IdLichHens != null && input.LocThongTin.IdLichHens.Any())
            {
                query = query.Where(x => input.LocThongTin.IdLichHens.Contains(x.IdLichHen));
            }

            var data = await query
                .Select(x => new tbLichHenExtend
                {
                    LichHen = x,
                })
           .OrderByDescending(x => x.LichHen.Stt)
           .ToListAsync();

            return data;
        }
        public async Task<DisplayModal_CRUD_LichHen_Output_Dto> DisplayModal_CRUD_LichHen(
          DisplayModal_CRUD_LichHen_Input_Dto input)
        {
            var lichHens = await Get_LichHens(input: new GetList_LichHen_Input_Dto
            {
                Loai = "single",
                LocThongTin = new LocThongTinDto
                {
                    IdLichHens = new List<Guid?> { input.IdLichHen }
                }
            });
            var bacSys = await _bacSyRepo.Query()
                .Where(x => x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                  && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung
                  && x.TrangThaiLamViec == (int?)TrangThaiLamViecEnum.DangLamViec)
                .ToListAsync() ?? new List<tbBacSy>();

            var output = new DisplayModal_CRUD_LichHen_Output_Dto
            {
                Loai = input.Loai,
                BacSys = bacSys,
                LichHen = lichHens.FirstOrDefault() ?? new tbLichHenExtend()
                {
                    LichHen = new tbLichHen()
                },
            };
            return output;
        }
        public async Task Create_LichHen(tbLichHenExtend lichHen)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _lichHen = new tbLichHen
                {
                    IdLichHen = Guid.NewGuid(),
                    IdBenhNhan = lichHen.LichHen.IdBenhNhan,
                    IdBacSy = lichHen.LichHen.IdBacSy,
                    NgayHen = lichHen.LichHen.NgayHen,
                    ThoiGianBatDau = lichHen.LichHen.ThoiGianBatDau,
                    ThoiGianKetThuc = lichHen.LichHen.ThoiGianKetThuc,
                    LoaiDieuTri = lichHen.LichHen.LoaiDieuTri,
                    LoaiThoiGian = lichHen.LichHen.LoaiThoiGian,
                    TrangThaiKham = (int?)LichHenTrangThaiKhamEnum.ChuaDenHen,
                    LyDoHuyHen = lichHen.LichHen.LyDoHuyHen,
                    NoiDungKham = lichHen.LichHen.NoiDungKham,
                    GhiChu = lichHen.LichHen.GhiChu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now
                };
                await _unitOfWork.InsertAsync<tbLichHen, Guid>(_lichHen);
            });
        }
        public async Task Update_LichHen(tbLichHenExtend lichHen)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _lichHen = await _lichHenRepo.GetByIdAsync(lichHen.LichHen.IdLichHen);

                if (_lichHen == null)
                    throw new Exception("Bệnh nhân không tồn tại.");

                // Cập nhật thông tin chính
                {
                    _lichHen.IdBenhNhan = lichHen.LichHen.IdBenhNhan;
                    _lichHen.IdBacSy = lichHen.LichHen.IdBacSy;
                    _lichHen.NgayHen = lichHen.LichHen.NgayHen;
                    _lichHen.ThoiGianBatDau = lichHen.LichHen.ThoiGianBatDau;
                    _lichHen.ThoiGianKetThuc = lichHen.LichHen.ThoiGianKetThuc;
                    _lichHen.LoaiDieuTri = lichHen.LichHen.LoaiDieuTri;
                    _lichHen.LoaiThoiGian = lichHen.LichHen.LoaiThoiGian;
                    _lichHen.LyDoHuyHen = lichHen.LichHen.LyDoHuyHen;
                    _lichHen.NoiDungKham = lichHen.LichHen.NoiDungKham;
                    _lichHen.GhiChu = lichHen.LichHen.GhiChu;

                    _lichHen.NgaySua = DateTime.Now;
                    _lichHen.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbLichHen, Guid>(_lichHen);
                }
                ;
            });
        }
        public async Task Delete_LichHens(List<Guid> idLichHens)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var lichHens_DELETE = await _lichHenRepo.Query()
                    .Where(x => idLichHens.Contains(x.IdLichHen))
                    .ToListAsync();

                foreach (var _lichHen in lichHens_DELETE)
                {
                    _lichHen.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _lichHen.NgaySua = DateTime.Now;
                    _lichHen.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbLichHen, Guid>(_lichHen);

                    //var nguoiThans_OLD = await _lichHenNguoiThanRepo.Query()
                    //  .Where(x => x.IdLichHen == _lichHen.IdLichHen)
                    //  .ToListAsync(); // Danh sách hiện có

                    //foreach (var nguoiThan in nguoiThans_OLD)
                    //{
                    //    nguoiThan.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    //    nguoiThan.NgaySua = DateTime.Now;
                    //    nguoiThan.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                    //    await _unitOfWork.UpdateAsync<tbLichHenNguoiThan, Guid>(nguoiThan);
                    //}
                    //;

                    // Xóa lịch hẹn + bệnh án ... nếu có
                }
            });
        }
        #endregion
    }
}