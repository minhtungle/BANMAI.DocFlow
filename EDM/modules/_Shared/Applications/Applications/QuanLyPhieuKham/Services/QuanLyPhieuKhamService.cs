using Applications.QuanLyPhieuKham.Dtos;
using Applications.QuanLyPhieuKham.Filters;
using Applications.QuanLyPhieuKham.Interfaces;
using Applications.QuanLyPhieuKham.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Enums;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyPhieuKham.Services
{
    public class QuanLyPhieuKhamService : BaseService, IQuanLyPhieuKhamService
    {
        private readonly IRepository<tbPhieuKham, Guid> _phieuKhamRepo;
        private readonly IRepository<tbBacSy, Guid> _bacSyRepo;
        private readonly IRepository<tbTienTrinhDieuTri, Guid> _tienTrinhDieuTriRepo;
        public QuanLyPhieuKhamService(
          IUserContext userContext,
          IUnitOfWork unitOfWork,

          IRepository<tbPhieuKham, Guid> phieuKhamRepo,
          IRepository<tbBacSy, Guid> bacSyRepo,
          IRepository<tbTienTrinhDieuTri, Guid> tienTrinhDieuTriRepo
          ) : base(userContext, unitOfWork)
        {
            _phieuKhamRepo = phieuKhamRepo;
            _bacSyRepo = bacSyRepo;
            _tienTrinhDieuTriRepo = tienTrinhDieuTriRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_Output_Dto> Index()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyPhieuKham");
            //var phieuKhams = await Get_PhieuKhams(input: new GetList_PhieuKham_Input_Dto());
            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }

        #region Phiếu khám
        public async Task<List<tbPhieuKhamExtend>> Get_PhieuKhams(GetList_PhieuKham_Input_Dto input)
        {
            var query = _phieuKhamRepo.Query()
                .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin.IdPhieuKhams != null && input.LocThongTin.IdPhieuKhams.Any())
            {
                query = query.Where(x => input.LocThongTin.IdPhieuKhams.Contains(x.IdPhieuKham));
            }

            var data = await query
                .Select(x => new tbPhieuKhamExtend
                {
                    PhieuKham = x,
                })
           .OrderByDescending(x => x.PhieuKham.Stt)
           .ToListAsync();

            return data;
        }
        public async Task<DisplayModal_CRUD_PhieuKham_Output_Dto> DisplayModal_CRUD_PhieuKham(
          DisplayModal_CRUD_PhieuKham_Input_Dto input)
        {
            var phieuKhams = await Get_PhieuKhams(input: new GetList_PhieuKham_Input_Dto
            {
                Loai = "single",
                LocThongTin = new LocThongTinDto
                {
                    IdPhieuKhams = new List<Guid?> { input.IdPhieuKham }
                }
            });
            var bacSys = await _bacSyRepo.Query()
                .Where(x => x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                  && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung
                  && x.TrangThaiLamViec == (int?)TrangThaiLamViecEnum.DangLamViec)
                .ToListAsync() ?? new List<tbBacSy>();

            var output = new DisplayModal_CRUD_PhieuKham_Output_Dto
            {
                Loai = input.Loai,
                BacSys = bacSys,
                PhieuKham = phieuKhams.FirstOrDefault() ?? new tbPhieuKhamExtend()
                {
                    PhieuKham = new tbPhieuKham()
                },
            };
            return output;
        }
        public async Task<XemChiTiet_PhieuKham_Output_Dto> XemChiTiet_PhieuKham(Guid idPhieuKham)
        {
            var phieuKham = _phieuKhamRepo.Query()
                .Where(pk => pk.IdPhieuKham == idPhieuKham && pk.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
               .GroupJoin(
                   _tienTrinhDieuTriRepo.Query(),
                   pk => pk.IdPhieuKham,
                   ttdt => ttdt.IdPhieuKham,
                   (pk, ttdts) => new XemChiTiet_PhieuKham_Output_Dto
                   {
                       PhieuKham = pk,
                       TienTrinhDieuTris = ttdts.ToList()
                   })
                .FirstOrDefault();

            return phieuKham;
        }
        public async Task Create_PhieuKham(tbPhieuKhamExtend phieuKham)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _phieuKham = new tbPhieuKham
                {
                    IdPhieuKham = Guid.NewGuid(),
                    //IdLichHen = phieuKham.PhieuKham.IdLichHen,
                    //IdBacSyDieuTri = phieuKham.PhieuKham.IdBacSyDieuTri,
                    //IdBacSyKham = phieuKham.PhieuKham.IdBacSyKham,
                    //NoiDungKham = phieuKham.PhieuKham.NoiDungKham,
                    //NoiDungDieuTri = phieuKham.PhieuKham.NoiDungDieuTri,
                    //LoaiKham = phieuKham.PhieuKham.LoaiKham,
                    GhiChu = phieuKham.PhieuKham.GhiChu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now
                };
                await _unitOfWork.InsertAsync<tbPhieuKham, Guid>(_phieuKham);
            });
        }
        public async Task Update_PhieuKham(tbPhieuKhamExtend phieuKham)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _phieuKham = await _phieuKhamRepo.GetByIdAsync(phieuKham.PhieuKham.IdPhieuKham);

                if (_phieuKham == null)
                    throw new Exception("Bệnh nhân không tồn tại.");

                // Cập nhật thông tin chính
                {
                    //_phieuKham.IdBacSyKham = phieuKham.PhieuKham.IdBacSyKham;
                    //_phieuKham.IdBacSyDieuTri = phieuKham.PhieuKham.IdBacSyDieuTri;
                    //_phieuKham.NoiDungKham = phieuKham.PhieuKham.NoiDungKham;
                    //_phieuKham.NoiDungDieuTri = phieuKham.PhieuKham.NoiDungDieuTri;
                    //_phieuKham.LoaiKham = phieuKham.PhieuKham.LoaiKham;
                    _phieuKham.GhiChu = phieuKham.PhieuKham.GhiChu;

                    _phieuKham.NgaySua = DateTime.Now;
                    _phieuKham.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbPhieuKham, Guid>(_phieuKham);
                }
                ;
            });
        }
        public async Task Delete_PhieuKhams(List<Guid> idPhieuKhams)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var phieuKhams_DELETE = await _phieuKhamRepo.Query()
                    .Where(x => idPhieuKhams.Contains(x.IdPhieuKham))
                    .ToListAsync();

                foreach (var _phieuKham in phieuKhams_DELETE)
                {
                    _phieuKham.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _phieuKham.NgaySua = DateTime.Now;
                    _phieuKham.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbPhieuKham, Guid>(_phieuKham);

                    //var nguoiThans_OLD = await _phieuKhamNguoiThanRepo.Query()
                    //  .Where(x => x.IdPhieuKham == _phieuKham.IdPhieuKham)
                    //  .ToListAsync(); // Danh sách hiện có

                    //foreach (var nguoiThan in nguoiThans_OLD)
                    //{
                    //    nguoiThan.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    //    nguoiThan.NgaySua = DateTime.Now;
                    //    nguoiThan.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                    //    await _unitOfWork.UpdateAsync<tbPhieuKhamNguoiThan, Guid>(nguoiThan);
                    //}
                    //;

                    // Xóa lịch hẹn + bệnh án ... nếu có
                }
            });
        }
        #endregion
    }
}