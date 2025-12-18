using Applications.QuanLyBenhNhan.Dtos;
using Applications.QuanLyBenhNhan.Filters;
using Applications.QuanLyBenhNhan.Interfaces;
using Applications.QuanLyBenhNhan.Models;
using Applications.QuanLyLichDieuTri.Models;
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

namespace Applications.QuanLyBenhNhan.Services
{
    public class QuanLyBenhNhanService : BaseService, IQuanLyBenhNhanService
    {
        private readonly IRepository<tbBenhNhan, Guid> _benhNhanRepo;
        private readonly IRepository<tbBenhNhanNguoiThan, Guid> _benhNhanNguoiThanRepo;
        private readonly IRepository<tbLichHen, Guid> _lichHenRepo;
        private readonly IRepository<tbPhieuKham, Guid> _phieuKhamRepo;
        private readonly IRepository<tbLichDieuTri, Guid> _lichDieuTriRepo;
        private readonly IRepository<tbTienTrinhDieuTri, Guid> _tienTrinhDieuTriRepo;
        private readonly IRepository<tbBacSy, Guid> _bacSyRepo;
        private readonly IRepository<default_tbTinhTrangRang, Guid> _tinhTrangRangRepo;
        private readonly IRepository<tbThuThuat, Guid> _thuThuatRepo;
        public QuanLyBenhNhanService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,

            IRepository<tbBenhNhan, Guid> benhNhanRepo,
            IRepository<tbBenhNhanNguoiThan, Guid> benhNhanNguoiThanRepo,
            IRepository<tbLichHen, Guid> lichHenRepo,
            IRepository<tbPhieuKham, Guid> phieuKhamRepo,
            IRepository<tbLichDieuTri, Guid> lichDieuTriRepo,
            IRepository<tbTienTrinhDieuTri, Guid> tienTrinhDieuTriRepo,
            IRepository<tbBacSy, Guid> bacSyRepo,
            IRepository<default_tbTinhTrangRang, Guid> tinhTrangRangRepo,
            IRepository<tbThuThuat, Guid> thuThuatRepo
            ) : base(userContext, unitOfWork)
        {
            _benhNhanRepo = benhNhanRepo;
            _benhNhanNguoiThanRepo = benhNhanNguoiThanRepo;
            _lichHenRepo = lichHenRepo;
            _phieuKhamRepo = phieuKhamRepo;
            _lichDieuTriRepo = lichDieuTriRepo;
            _tienTrinhDieuTriRepo = tienTrinhDieuTriRepo;
            _bacSyRepo = bacSyRepo;
            _tinhTrangRangRepo = tinhTrangRangRepo;
            _thuThuatRepo = thuThuatRepo;

        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_Output_Dto> Index()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyBenhNhan");
            var benhNhans = await Get_BenhNhans(input: new GetList_BenhNhan_Input_Dto());
            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }

        #region Bệnh nhân
        public async Task<List<tbBenhNhanExtend>> Get_BenhNhans(GetList_BenhNhan_Input_Dto input)
        {
            var query = _benhNhanRepo.Query()
                .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin.IdBenhNhans != null && input.LocThongTin.IdBenhNhans.Any())
            {
                query = query.Where(x => input.LocThongTin.IdBenhNhans.Contains(x.IdBenhNhan));
            }

            var data = await query
                .Select(x => new tbBenhNhanExtend
                {
                    BenhNhan = x,
                })
           .OrderByDescending(x => x.BenhNhan.Stt)
           .ToListAsync();

            return data;
        }
        public async Task<DisplayModal_CRUD_BenhNhan_Output_Dto> DisplayModal_CRUD_BenhNhan(
            DisplayModal_CRUD_BenhNhan_Input_Dto input)
        {
            var benhNhans = await Get_BenhNhans(input: new GetList_BenhNhan_Input_Dto
            {
                Loai = "single",
                LocThongTin = new LocThongTinDto
                {
                    IdBenhNhans = new List<Guid?> { input.IdBenhNhan }
                }
            });
            var output = new DisplayModal_CRUD_BenhNhan_Output_Dto
            {
                Loai = input.Loai,
                BenhNhan = benhNhans.FirstOrDefault() ?? new tbBenhNhanExtend()
                {
                    BenhNhan = new tbBenhNhan()
                },
            };
            return output;
        }
        public async Task<bool> IsExisted_BenhNhan(tbBenhNhan benhNhan)
        {
            var benhNhan_OLD = await _benhNhanRepo.Query()
                .FirstOrDefaultAsync(x =>
                    x.HoTen == benhNhan.HoTen
                    && (x.SoDienThoai == benhNhan.SoDienThoai || x.CCCD == benhNhan.CCCD)
                    && x.IdBenhNhan != benhNhan.IdBenhNhan
                    && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return benhNhan_OLD != null;
        }
        public async Task Create_BenhNhan(tbBenhNhanExtend benhNhan)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _benhNhan = new tbBenhNhan
                {
                    IdBenhNhan = Guid.NewGuid(),
                    HoTen = benhNhan.BenhNhan.HoTen,
                    GioiTinh = benhNhan.BenhNhan.GioiTinh,
                    NgaySinh = benhNhan.BenhNhan.NgaySinh,
                    SoDienThoai = benhNhan.BenhNhan.SoDienThoai,
                    Email = benhNhan.BenhNhan.Email,
                    CCCD = benhNhan.BenhNhan.CCCD,
                    NgheNghiep = benhNhan.BenhNhan.NgheNghiep,
                    DiaChi = benhNhan.BenhNhan.DiaChi,
                    TienSuBenh = benhNhan.BenhNhan.TienSuBenh,
                    GhiChu = benhNhan.BenhNhan.GhiChu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now
                };
                await _unitOfWork.InsertAsync<tbBenhNhan, Guid>(_benhNhan);

                foreach (var nguoiThan in benhNhan.NguoiThans)
                {
                    var _nguoiThan = new tbBenhNhanNguoiThan
                    {
                        IdBenhNhan = Guid.NewGuid(),
                        QuanHeVoiBenhNhan = nguoiThan.QuanHeVoiBenhNhan,
                        HoTen = nguoiThan.HoTen,
                        GioiTinh = nguoiThan.GioiTinh,
                        NgaySinh = nguoiThan.NgaySinh,
                        SoDienThoai = nguoiThan.SoDienThoai,
                        Email = nguoiThan.Email,
                        CCCD = nguoiThan.CCCD,
                        NgheNghiep = nguoiThan.NgheNghiep,
                        DiaChi = nguoiThan.DiaChi,
                        GhiChu = nguoiThan.GhiChu,

                        TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                        IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                        NgayTao = DateTime.Now,
                    };
                    await _unitOfWork.InsertAsync<tbBenhNhanNguoiThan, Guid>(_nguoiThan);
                }
                ;

                // Một bệnh nhân mặc định có 1 phiếu khám
                var _phieuKham = new tbPhieuKham
                {
                    IdPhieuKham = Guid.NewGuid(),
                    IdBenhNhan = _benhNhan.IdBenhNhan,
                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now
                };
                await _unitOfWork.InsertAsync<tbPhieuKham, Guid>(_phieuKham);
            });
        }
        public async Task Update_BenhNhan(tbBenhNhanExtend benhNhan)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _benhNhan = await _benhNhanRepo.GetByIdAsync(benhNhan.BenhNhan.IdBenhNhan);

                if (_benhNhan == null)
                    throw new Exception("Bệnh nhân không tồn tại.");

                // Cập nhật thông tin chính
                {
                    _benhNhan.HoTen = benhNhan.BenhNhan.HoTen;
                    _benhNhan.GioiTinh = benhNhan.BenhNhan.GioiTinh;
                    _benhNhan.NgaySinh = benhNhan.BenhNhan.NgaySinh;
                    _benhNhan.SoDienThoai = benhNhan.BenhNhan.SoDienThoai;
                    _benhNhan.Email = benhNhan.BenhNhan.Email;
                    _benhNhan.CCCD = benhNhan.BenhNhan.CCCD;
                    _benhNhan.NgheNghiep = benhNhan.BenhNhan.NgheNghiep;
                    _benhNhan.DiaChi = benhNhan.BenhNhan.DiaChi;
                    _benhNhan.TienSuBenh = benhNhan.BenhNhan.TienSuBenh;
                    _benhNhan.GhiChu = benhNhan.BenhNhan.GhiChu;

                    _benhNhan.NgaySua = DateTime.Now;
                    _benhNhan.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbBenhNhan, Guid>(_benhNhan);
                }
                ;

                // Cập nhật thông tin người thân
                {
                    var nguoiThans_OLD = await _benhNhanNguoiThanRepo.Query()
                        .Where(x => x.IdBenhNhan == _benhNhan.IdBenhNhan)
                        .ToListAsync(); // Danh sách hiện có

                    var nguoiThans_DELETE = nguoiThans_OLD
                        .Where(x => !benhNhan.NguoiThans.Any(y => y.IdBenhNhan == x.IdBenhNhan))
                        .ToList(); // Danh sách cần xóa

                    foreach (var nguoiThan in nguoiThans_DELETE)
                    {
                        await _unitOfWork.DeleteAsync<tbBenhNhanNguoiThan, Guid>(nguoiThan);
                    }
                    ;

                    foreach (var nguoiThan in benhNhan.NguoiThans)
                    {
                        var newData = nguoiThans_OLD
                            .FirstOrDefault(y => y.IdBenhNhan == nguoiThan.IdBenhNhan);
                        if (newData != null)
                        {
                            nguoiThan.QuanHeVoiBenhNhan = newData.QuanHeVoiBenhNhan;
                            nguoiThan.HoTen = newData.HoTen;
                            nguoiThan.GioiTinh = newData.GioiTinh;
                            nguoiThan.NgaySinh = newData.NgaySinh;
                            nguoiThan.SoDienThoai = newData.SoDienThoai;
                            nguoiThan.Email = newData.Email;
                            nguoiThan.CCCD = newData.CCCD;
                            nguoiThan.NgheNghiep = newData.NgheNghiep;
                            nguoiThan.DiaChi = newData.DiaChi;
                            nguoiThan.GhiChu = newData.GhiChu;

                            nguoiThan.NgaySua = DateTime.Now;
                            nguoiThan.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                            await _unitOfWork.UpdateAsync<tbBenhNhanNguoiThan, Guid>(nguoiThan);
                        }
                        else
                        {
                            var _nguoiThan = new tbBenhNhanNguoiThan
                            {
                                IdBenhNhan = Guid.NewGuid(),
                                QuanHeVoiBenhNhan = nguoiThan.QuanHeVoiBenhNhan,
                                HoTen = nguoiThan.HoTen,
                                GioiTinh = nguoiThan.GioiTinh,
                                NgaySinh = nguoiThan.NgaySinh,
                                SoDienThoai = nguoiThan.SoDienThoai,
                                Email = nguoiThan.Email,
                                CCCD = nguoiThan.CCCD,
                                NgheNghiep = nguoiThan.NgheNghiep,
                                DiaChi = nguoiThan.DiaChi,
                                GhiChu = nguoiThan.GhiChu,

                                TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                                MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                                IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                            };
                            await _unitOfWork.InsertAsync<tbBenhNhanNguoiThan, Guid>(_nguoiThan);
                        }
                    }
                    ;
                }
                ;
            });
        }
        public async Task Delete_BenhNhans(List<Guid> idBenhNhans)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var benhNhans_DELETE = await _benhNhanRepo.Query()
                    .Where(x => idBenhNhans.Contains(x.IdBenhNhan))
                    .ToListAsync();

                foreach (var _benhNhan in benhNhans_DELETE)
                {
                    _benhNhan.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _benhNhan.NgaySua = DateTime.Now;
                    _benhNhan.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbBenhNhan, Guid>(_benhNhan);

                    var nguoiThans_OLD = await _benhNhanNguoiThanRepo.Query()
                      .Where(x => x.IdBenhNhan == _benhNhan.IdBenhNhan)
                      .ToListAsync(); // Danh sách hiện có

                    foreach (var nguoiThan in nguoiThans_OLD)
                    {
                        nguoiThan.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                        nguoiThan.NgaySua = DateTime.Now;
                        nguoiThan.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                        await _unitOfWork.UpdateAsync<tbBenhNhanNguoiThan, Guid>(nguoiThan);
                    }
                    ;

                    // Xóa lịch hẹn + bệnh án ... nếu có
                }
            });
        }
        #endregion

        #region Xem chi tiết
        public async Task<XemChiTiet_BenhNhan_Output_Dto> XemChiTiet_BenhNhan(Guid idBenhNhan)
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyBenhNhan");
            var benhNhans = await Get_BenhNhans(input: new GetList_BenhNhan_Input_Dto()) ?? new List<tbBenhNhanExtend>();
            var benhNhan = benhNhans.FirstOrDefault(x => x.BenhNhan.IdBenhNhan == idBenhNhan) ?? new tbBenhNhanExtend();
            var output = new XemChiTiet_BenhNhan_Output_Dto
            {
                ThaoTacs = thaoTacs,
                BenhNhans = benhNhans,
                BenhNhan = benhNhan,
            };
            return output;
        }
        public async Task<ShowTab_ThongTinCoBan_Output_Dto> ShowTab_ThongTinCoBan_BenhNhan(Guid idBenhNhan)
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyBenhNhan");
            var benhNhans = await Get_BenhNhans(input: new GetList_BenhNhan_Input_Dto
            {
                Loai = "single",
                LocThongTin = new LocThongTinDto
                {
                    IdBenhNhans = new List<Guid?> { idBenhNhan }
                }
            });
            var output = new ShowTab_ThongTinCoBan_Output_Dto
            {
                ThaoTacs = thaoTacs,
                BenhNhan = benhNhans.FirstOrDefault() ?? new tbBenhNhanExtend()
                {
                    BenhNhan = new tbBenhNhan()
                },
            };
            return output;
        }
        public async Task<ShowTab_LichHen_Output_Dto> ShowTab_LichHen_BenhNhan(Guid idBenhNhan)
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyBenhNhan");
            var lichHens = await _lichHenRepo.Query()
                .Where(x =>
                x.IdBenhNhan == idBenhNhan
                && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .OrderByDescending(x => x.NgayHen)
                .ToListAsync();

            var output = new ShowTab_LichHen_Output_Dto
            {
                ThaoTacs = thaoTacs,
                LichHens = lichHens,
            };
            return output;
        }
        public async Task<ShowTab_PhieuKham_Output_Dto> ShowTab_PhieuKham_BenhNhan(Guid idBenhNhan)
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyBenhNhan");

            // 1) Lấy phiếu + lịch điều trị
            var baseData = await _phieuKhamRepo.Query().AsNoTracking()
                .Where(pk => pk.IdBenhNhan == idBenhNhan
                    && pk.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                    && pk.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .Select(pk => new ShowTab_PhieuKham_Output_Dto
                {
                    PhieuKham = pk,
                })
                .FirstOrDefaultAsync();

            if (baseData == null)
                return new ShowTab_PhieuKham_Output_Dto { ThaoTacs = thaoTacs };

            baseData.LichDieuTris = await _lichDieuTriRepo.Query().AsNoTracking()
                        .Where(ldt => ldt.IdPhieuKham == baseData.PhieuKham.IdPhieuKham
                            && ldt.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung)
                        .Select(x => new tbLichDieuTriExtend
                        {
                            LichDieuTri = x,
                        })
                        .OrderByDescending(x => x.LichDieuTri.NgayDieuTri) // tuỳ field
                        .ToListAsync();

            var lichIds = baseData.LichDieuTris.Select(x => x.LichDieuTri.IdLichDieuTri).ToList();

            var allTienTrinhs =
            await (from tt in _tienTrinhDieuTriRepo.Query().AsNoTracking()
                   where tt.IdLichDieuTri.HasValue
                         && lichIds.Contains(tt.IdLichDieuTri.Value)
                         && tt.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung

                   // LEFT JOIN bác sỹ điều trị
                   join bsDieuTri in _bacSyRepo.Query().AsNoTracking()
                       on tt.IdBacSy equals bsDieuTri.IdBacSy into gBsDieuTri
                   from bsDieuTri in gBsDieuTri.DefaultIfEmpty()

                   // LEFT JOIN phụ tá
                   join phuTa in _bacSyRepo.Query().AsNoTracking()
                       on tt.IdPhuTa equals phuTa.IdBacSy into gPhuTa
                   from phuTa in gPhuTa.DefaultIfEmpty()

                   // LEFT JOIN tình trạng răng
                   join tinhTrangRang in _tinhTrangRangRepo.Query().AsNoTracking()
                       on tt.IdTinhTrangRang equals tinhTrangRang.IdTinhTrangRang into gTinhTrangRang
                   from tinhTrangRang in gTinhTrangRang.DefaultIfEmpty()
                   
                   // LEFT JOIN thủ thuật
                   join thuThuat in _thuThuatRepo.Query().AsNoTracking()
                       on tt.IdThuThuat equals thuThuat.IdThuThuat into gThuThuat
                   from thuThuat in gThuThuat.DefaultIfEmpty()

                   orderby tt.Stt descending
                   select new tbTienTrinhDieuTriExtend
                   {
                       TienTrinhDieuTri = tt,
                       BacSyDieuTri = bsDieuTri,  // có thể null
                       PhuTa = phuTa,         // có thể null
                       TinhTrangRang = tinhTrangRang,
                       ThuThuat = thuThuat,
                   })
            .ToListAsync();

            // 1) Lookup: IdLichDieuTri -> List<TienTrinh>
            var ttLookup = allTienTrinhs
                .Where(t => t.TienTrinhDieuTri?.IdLichDieuTri != null)                 // tránh null
                .GroupBy(t => t.TienTrinhDieuTri.IdLichDieuTri.Value)                 // Guid? -> Guid
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(t => new tbTienTrinhDieuTriExtend                    // clone an toàn
                    {
                        TienTrinhDieuTri = t.TienTrinhDieuTri,
                        BacSyDieuTri = t.BacSyDieuTri,
                        PhuTa = t.PhuTa,
                        TinhTrangRang = t.TinhTrangRang,
                        ThuThuat = t.ThuThuat
                    }).ToList()
                );

            // 2) Build result an toàn null (baseData hoặc LichDieuTris có thể null)
            var result = new ShowTab_PhieuKham_Output_Dto
            {
                PhieuKham = baseData?.PhieuKham,
                ThaoTacs = thaoTacs,
                LichDieuTris = (baseData?.LichDieuTris ?? new List<tbLichDieuTriExtend>())
                    .Select(ldt =>
                    {
                        var idLdt = ldt?.LichDieuTri?.IdLichDieuTri ?? Guid.Empty;     // phòng null
                        ttLookup.TryGetValue(idLdt, out var ttList);
                        return new tbLichDieuTriExtend
                        {
                            LichDieuTri = ldt.LichDieuTri,
                            TienTrinhDieuTris = ttList ?? new List<tbTienTrinhDieuTriExtend>(),
                            //AnhMoTas = (anhLookup.TryGetValue(idLdt, out var anh) ? anh : new List<tbLichDieuTri_AnhMoTa>())
                        };
                    })
                    .ToList()
            };


            return result;
        }

        #endregion
    }
}