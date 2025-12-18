using Applications.QuanLyLichDieuTri.Dtos;
using Applications.QuanLyLichDieuTri.Filters;
using Applications.QuanLyLichDieuTri.Interfaces;
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
using System.Web;

namespace Applications.QuanLyLichDieuTri.Services
{
    public class QuanLyLichDieuTriService : BaseService, IQuanLyLichDieuTriService
    {
        private readonly IRepository<tbLichDieuTri, Guid> _lichDieuTriRepo;
        private readonly IRepository<tbBacSy, Guid> _bacSyRepo;
        private readonly IRepository<tbTienTrinhDieuTri, Guid> _tienTrinhDieuTriRepo;
        private readonly IRepository<tbLoaiThuThuat, Guid> _loaiThuThuatRepo;
        private readonly IRepository<tbThuThuat, Guid> _thuThuatRepo;
        private readonly IRepository<default_tbTinhTrangRang, Guid> _tinhTrangRangRepo;
        private readonly IRepository<default_tbRang, Guid> _soDoHamRangRepo;
        public QuanLyLichDieuTriService(
          IUserContext userContext,
          IUnitOfWork unitOfWork,

          IRepository<tbLichDieuTri, Guid> lichDieuTriRepo,
          IRepository<tbBacSy, Guid> bacSyRepo,
          IRepository<tbTienTrinhDieuTri, Guid> tienTrinhDieuTriRepo,
          IRepository<tbLoaiThuThuat, Guid> loaiThuThuatRepo,
          IRepository<tbThuThuat, Guid> thuThuatRepo,
          IRepository<default_tbTinhTrangRang, Guid> tinhTrangRangRepo,
          IRepository<default_tbRang, Guid> soDoHamRangRepo
          ) : base(userContext, unitOfWork)
        {
            _lichDieuTriRepo = lichDieuTriRepo;
            _bacSyRepo = bacSyRepo;
            _tienTrinhDieuTriRepo = tienTrinhDieuTriRepo;
            _loaiThuThuatRepo = loaiThuThuatRepo;
            _thuThuatRepo = thuThuatRepo;
            _tinhTrangRangRepo = tinhTrangRangRepo;
            _soDoHamRangRepo = soDoHamRangRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_Output_Dto> Index()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyLichDieuTri");
            //var lichDieuTris = await Get_LichDieuTris(input: new GetList_LichDieuTri_Input_Dto());
            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }

        #region Lịch điều trị
        public async Task<List<tbLichDieuTriExtend>> Get_LichDieuTris(GetList_LichDieuTri_Input_Dto input)
        {
            var query = _lichDieuTriRepo.Query()
                .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin.IdLichDieuTris != null && input.LocThongTin.IdLichDieuTris.Any())
            {
                query = query.Where(x => input.LocThongTin.IdLichDieuTris.Contains(x.IdLichDieuTri));
            }

            var data = await query
                .Select(x => new tbLichDieuTriExtend
                {
                    LichDieuTri = x,
                })
           .OrderByDescending(x => x.LichDieuTri.Stt)
           .ToListAsync();

            return data;
        }
        public async Task<DisplayModal_CRUD_LichDieuTri_Output_Dto> DisplayModal_CRUD_LichDieuTri(
          DisplayModal_CRUD_LichDieuTri_Input_Dto input)
        {
            var lichDieuTri = await _lichDieuTriRepo.Query()
                .Where(x => x.IdLichDieuTri == input.IdLichDieuTri
                    && x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                    && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .GroupJoin(
                _tienTrinhDieuTriRepo.Query(),
                ldt => ldt.IdLichDieuTri,
                ttdt => ttdt.IdLichDieuTri,
                (ldt, ttdts) => new tbLichDieuTriExtend
                {
                    LichDieuTri = ldt,
                    TienTrinhDieuTris = ttdts.Select(x => new tbTienTrinhDieuTriExtend
                    {
                        TienTrinhDieuTri = x
                    }).ToList(),
                })
                .FirstOrDefaultAsync();

            var loaiThuThuats = await _loaiThuThuatRepo.Query()
                .Where(x => x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .GroupJoin(
                    _thuThuatRepo.Query()
                    .Where(tt => tt.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                            && tt.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung),
                    ltt => ltt.IdLoaiThuThuat,
                    tt => tt.IdLoaiThuThuat, // đang kiểu string
                    (ltt, tts) => new tbLoaiThuThuatExtend
                    {
                        LoaiThuThuat = ltt,
                        ThuThuats = tts.ToList()
                    })
                .ToListAsync() ?? new List<tbLoaiThuThuatExtend>();

            var bacSys = await _bacSyRepo.Query()
                .Where(x => x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                  && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung
                  && x.TrangThaiLamViec == (int?)TrangThaiLamViecEnum.DangLamViec)
                .ToListAsync() ?? new List<tbBacSy>();

            var tinhTrangRangs = await _tinhTrangRangRepo.Query()
                .Where(x => x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung)
                .ToListAsync() ?? new List<default_tbTinhTrangRang>();

            var output = new DisplayModal_CRUD_LichDieuTri_Output_Dto
            {
                Loai = input.Loai,
                LichDieuTri = lichDieuTri ?? new tbLichDieuTriExtend()
                {
                    //TienTrinhDieuTris = new List<tbTienTrinhDieuTriExtend>()
                    //{
                    //    new tbTienTrinhDieuTriExtend(), // Tạo 1 bản ghi mẫu
                    //}
                },
                LoaiThuThuats = loaiThuThuats,
                BacSys = bacSys,
                TinhTrangRangs = tinhTrangRangs
            };
            return output;
        }
        public async Task<tbThuThuat> ChonThuThuat(
          Guid idThuThuat)
        {
            var output = await _thuThuatRepo.GetByIdAsync(idThuThuat) ?? new tbThuThuat();
            return output;
        }
        public async Task<DisplayModal_ChonRang_Output_Dto> DisplayModal_ChonRang(
          DisplayModal_ChonRang_Input_Dto input)
        {
            var soDoHamRangs = await _soDoHamRangRepo.Query()
                .Where(x => x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung)
                .ToListAsync();

            var soDoHamRang_NguoiLon = new SoDoHamRangDto
            {
                HamTren_BenPhai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.NguoiLon && rang.ViTri >= 11 && rang.ViTri <= 18)
                        .OrderByDescending(rang => rang.ViTri)
                        .ToList(),
                HamTren_BenTrai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.NguoiLon && rang.ViTri >= 21 && rang.ViTri <= 28)
                        .OrderBy(rang => rang.ViTri)
                        .ToList(),
                HamDuoi_BenPhai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.NguoiLon && rang.ViTri >= 31 && rang.ViTri <= 38)
                        .OrderByDescending(rang => rang.ViTri)
                        .ToList(),
                HamDuoi_BenTrai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.NguoiLon && rang.ViTri >= 41 && rang.ViTri <= 48)
                        .OrderBy(rang => rang.ViTri)
                        .ToList(),
            };

            var soDoHamRang_TreEm = new SoDoHamRangDto
            {
                HamTren_BenPhai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.TreEm && rang.ViTri >= 51 && rang.ViTri <= 55)
                        .OrderByDescending(rang => rang.ViTri)
                        .ToList(),
                HamTren_BenTrai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.TreEm && rang.ViTri >= 61 && rang.ViTri <= 65)
                        .OrderBy(rang => rang.ViTri)
                        .ToList(),
                HamDuoi_BenPhai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.TreEm && rang.ViTri >= 71 && rang.ViTri <= 75)
                        .OrderByDescending(rang => rang.ViTri)
                        .ToList(),
                HamDuoi_BenTrai = soDoHamRangs
                        .Where(rang => rang.LoaiRang == (int)LoaiRangEnum.TreEm && rang.ViTri >= 81 && rang.ViTri <= 85)
                        .OrderBy(rang => rang.ViTri)
                        .ToList(),
            };

            var output = new DisplayModal_ChonRang_Output_Dto
            {
                IdTienTrinhDieuTri = input.IdTienTrinhDieuTri,
                SoDoHamRang_NguoiLon = soDoHamRang_NguoiLon,
                SoDoHamRang_TreEm = soDoHamRang_TreEm,
                SoRangDaChons = input.SoRangDaChons ?? new List<int>()
            };
            return output;
        }
        public async Task CapNhat_TongChiPhi_LichDieuTri(Guid idLichDieuTri)
        {
            var lichDieuTri_OLD = await _lichDieuTriRepo.GetByIdAsync(idLichDieuTri);
            if (lichDieuTri_OLD != null)
            {
                var tienTrinhDieuTris = await _tienTrinhDieuTriRepo.Query()
                    .Where(x => x.IdLichDieuTri == idLichDieuTri
                    && x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
                    && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                    .ToListAsync();
                long tongChiPhi = 0;
                foreach (var tienTrinhDieuTri in tienTrinhDieuTris)
                {
                    tongChiPhi += tienTrinhDieuTri.TongChiPhi ?? 0;
                }
                // Cập nhật tổng chi phí vào lịch điều trị
                lichDieuTri_OLD.TongChiPhi = tongChiPhi;
                lichDieuTri_OLD.NgaySua = DateTime.Now;
                lichDieuTri_OLD.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                await _unitOfWork.UpdateAsync<tbLichDieuTri, Guid>(lichDieuTri_OLD);
            }
        }
        public async Task<bool> IsExisted_LichDieuTri(tbLichDieuTri lichDieuTri)
        {
            var lichDieuTri_OLD = await _lichDieuTriRepo.Query()
                .FirstOrDefaultAsync(x => x.NgayDieuTri == lichDieuTri.NgayDieuTri
                && x.IdLichDieuTri != lichDieuTri.IdLichDieuTri
                && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return lichDieuTri_OLD != null;
        }
        public async Task Create_LichDieuTri(tbLichDieuTriExtend lichDieuTri)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _lichDieuTri = new tbLichDieuTri
                {
                    IdLichDieuTri = Guid.NewGuid(),
                    IdPhieuKham = lichDieuTri.LichDieuTri.IdPhieuKham,
                    TongChiPhi = 0,
                    NgayDieuTri = lichDieuTri.LichDieuTri.NgayDieuTri,
                    GhiChu = lichDieuTri.LichDieuTri.GhiChu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now
                };

                foreach (var tienTrinhDieuTri in lichDieuTri.TienTrinhDieuTris)
                {
                    // Tính tiền
                    var tongChiPhi = (tienTrinhDieuTri.TienTrinhDieuTri.GiaTien ?? 0)
                    * (100 - (tienTrinhDieuTri.TienTrinhDieuTri.PhanTramGiamGia ?? 0)) / 100
                    * (tienTrinhDieuTri.TienTrinhDieuTri.SoLuong ?? 0);
                    _lichDieuTri.TongChiPhi += (long)tongChiPhi; // Cap nhat tong chi phi luon

                    var _tienTrinhDieuTri = new tbTienTrinhDieuTri
                    {
                        IdTienTrinhDieuTri = Guid.NewGuid(),
                        IdPhieuKham = _lichDieuTri.IdPhieuKham,
                        IdLichDieuTri = _lichDieuTri.IdLichDieuTri,
                        IdBacSy = tienTrinhDieuTri.TienTrinhDieuTri.IdBacSy,
                        IdPhuTa = tienTrinhDieuTri.TienTrinhDieuTri.IdPhuTa,
                        RangSo = tienTrinhDieuTri.TienTrinhDieuTri.RangSo,
                        IdTinhTrangRang = tienTrinhDieuTri.TienTrinhDieuTri.IdTinhTrangRang,
                        IdThuThuat = tienTrinhDieuTri.TienTrinhDieuTri.IdThuThuat,
                        NoiDungThuThuat = tienTrinhDieuTri.TienTrinhDieuTri.NoiDungThuThuat,
                        SoLuong = tienTrinhDieuTri.TienTrinhDieuTri.SoLuong,
                        GiaTien = tienTrinhDieuTri.TienTrinhDieuTri.GiaTien,
                        PhanTramGiamGia = tienTrinhDieuTri.TienTrinhDieuTri.PhanTramGiamGia,
                        TongChiPhi = (long)tongChiPhi,
                        TrangThaiDieuTri = tienTrinhDieuTri.TienTrinhDieuTri.TrangThaiDieuTri,

                        GhiChu = tienTrinhDieuTri.TienTrinhDieuTri.GhiChu,
                        TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                        IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                        NgayTao = DateTime.Now
                    };
                    await _unitOfWork.InsertAsync<tbTienTrinhDieuTri, Guid>(_tienTrinhDieuTri);
                }
                ;

                await _unitOfWork.InsertAsync<tbLichDieuTri, Guid>(_lichDieuTri);
            });
        }
        public async Task Update_LichDieuTri(tbLichDieuTriExtend lichDieuTri)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _lichDieuTri = await _lichDieuTriRepo.GetByIdAsync(lichDieuTri.LichDieuTri.IdLichDieuTri);

                if (_lichDieuTri == null)
                    throw new Exception("Bản ghi không tồn tại.");

                // Cập nhật thông tin chính
                {
                    _lichDieuTri.GhiChu = lichDieuTri.LichDieuTri.GhiChu;

                    _lichDieuTri.NgaySua = DateTime.Now;
                    _lichDieuTri.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbLichDieuTri, Guid>(_lichDieuTri);

                    var tienTrinhDieuTris_OLD = await _tienTrinhDieuTriRepo.Query()
                      .Where(x => x.IdLichDieuTri == _lichDieuTri.IdLichDieuTri)
                      .ToListAsync(); // Danh sách hiện có

                    // Xử lý xóa các bản ghi cũ
                    foreach (var tienTrinhDieuTri_OLD in tienTrinhDieuTris_OLD)
                    {
                        tienTrinhDieuTri_OLD.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                        tienTrinhDieuTri_OLD.NgaySua = DateTime.Now;
                        tienTrinhDieuTri_OLD.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                        await _unitOfWork.UpdateAsync<tbTienTrinhDieuTri, Guid>(tienTrinhDieuTri_OLD);
                    }
                    ;

                    // Thêm các bản ghi mới
                    foreach (var tienTrinhDieuTri in lichDieuTri.TienTrinhDieuTris)
                    {
                        var isExisted = tienTrinhDieuTris_OLD
                            .Any(x => x.IdTienTrinhDieuTri == tienTrinhDieuTri.TienTrinhDieuTri.IdTienTrinhDieuTri);

                        // Tính tiền
                        var tongChiPhi = (tienTrinhDieuTri.TienTrinhDieuTri.GiaTien ?? 0)
                        * (100 - (tienTrinhDieuTri.TienTrinhDieuTri.PhanTramGiamGia ?? 0)) / 100
                        * (tienTrinhDieuTri.TienTrinhDieuTri.SoLuong ?? 0);

                        if (isExisted)
                        {
                            // Cập nhật lại bản ghi vừa xóa
                            var _tienTrinhDieuTri = tienTrinhDieuTris_OLD
                                .FirstOrDefault(x => x.IdTienTrinhDieuTri == tienTrinhDieuTri.TienTrinhDieuTri.IdTienTrinhDieuTri);

                            _tienTrinhDieuTri.IdBacSy = tienTrinhDieuTri.TienTrinhDieuTri.IdBacSy;
                            _tienTrinhDieuTri.IdPhuTa = tienTrinhDieuTri.TienTrinhDieuTri.IdPhuTa;
                            _tienTrinhDieuTri.RangSo = tienTrinhDieuTri.TienTrinhDieuTri.RangSo;
                            _tienTrinhDieuTri.IdTinhTrangRang = tienTrinhDieuTri.TienTrinhDieuTri.IdTinhTrangRang;
                            _tienTrinhDieuTri.IdThuThuat = tienTrinhDieuTri.TienTrinhDieuTri.IdThuThuat;
                            _tienTrinhDieuTri.NoiDungThuThuat = tienTrinhDieuTri.TienTrinhDieuTri.NoiDungThuThuat;
                            _tienTrinhDieuTri.SoLuong = tienTrinhDieuTri.TienTrinhDieuTri.SoLuong;
                            _tienTrinhDieuTri.GiaTien = tienTrinhDieuTri.TienTrinhDieuTri.GiaTien;
                            _tienTrinhDieuTri.PhanTramGiamGia = tienTrinhDieuTri.TienTrinhDieuTri.PhanTramGiamGia;
                            _tienTrinhDieuTri.TongChiPhi = (long)tongChiPhi;
                            _tienTrinhDieuTri.TrangThaiDieuTri = tienTrinhDieuTri.TienTrinhDieuTri.TrangThaiDieuTri;
                            _tienTrinhDieuTri.GhiChu = tienTrinhDieuTri.TienTrinhDieuTri.GhiChu;

                            _tienTrinhDieuTri.TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung;
                            _tienTrinhDieuTri.NgaySua = DateTime.Now;
                            _tienTrinhDieuTri.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                            await _unitOfWork.UpdateAsync<tbTienTrinhDieuTri, Guid>(_tienTrinhDieuTri);
                        }
                        else
                        {
                            var _tienTrinhDieuTri = new tbTienTrinhDieuTri
                            {
                                IdTienTrinhDieuTri = Guid.NewGuid(),
                                IdPhieuKham = _lichDieuTri.IdLichDieuTri,
                                IdLichDieuTri = _lichDieuTri.IdLichDieuTri,
                                IdBacSy = tienTrinhDieuTri.TienTrinhDieuTri.IdBacSy,
                                IdPhuTa = tienTrinhDieuTri.TienTrinhDieuTri.IdPhuTa,
                                RangSo = tienTrinhDieuTri.TienTrinhDieuTri.RangSo,
                                IdTinhTrangRang = tienTrinhDieuTri.TienTrinhDieuTri.IdTinhTrangRang,
                                IdThuThuat = tienTrinhDieuTri.TienTrinhDieuTri.IdThuThuat,
                                NoiDungThuThuat = tienTrinhDieuTri.TienTrinhDieuTri.NoiDungThuThuat,
                                SoLuong = tienTrinhDieuTri.TienTrinhDieuTri.SoLuong,
                                GiaTien = tienTrinhDieuTri.TienTrinhDieuTri.GiaTien,
                                PhanTramGiamGia = tienTrinhDieuTri.TienTrinhDieuTri.PhanTramGiamGia,
                                TongChiPhi = (long)tongChiPhi,
                                TrangThaiDieuTri = tienTrinhDieuTri.TienTrinhDieuTri.TrangThaiDieuTri,

                                GhiChu = tienTrinhDieuTri.TienTrinhDieuTri.GhiChu,
                                TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                                MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                                IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now
                            };
                            await _unitOfWork.InsertAsync<tbTienTrinhDieuTri, Guid>(_tienTrinhDieuTri);
                        }
                    }
                }
                ;

                await CapNhat_TongChiPhi_LichDieuTri(_lichDieuTri.IdLichDieuTri);
            });
        }
        public async Task Delete_LichDieuTris(List<Guid> idLichDieuTris)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var lichDieuTris_DELETE = await _lichDieuTriRepo.Query()
                    .Where(x => idLichDieuTris.Contains(x.IdLichDieuTri))
                    .ToListAsync();

                foreach (var _lichDieuTri in lichDieuTris_DELETE)
                {
                    _lichDieuTri.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _lichDieuTri.NgaySua = DateTime.Now;
                    _lichDieuTri.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbLichDieuTri, Guid>(_lichDieuTri);

                    var tienTrinhDieuTris_DELETE = await _tienTrinhDieuTriRepo.Query()
                        .Where(x => x.IdLichDieuTri == _lichDieuTri.IdLichDieuTri)
                        .ToListAsync();
                    foreach (var _tienTrinhDieuTri in tienTrinhDieuTris_DELETE)
                    {
                        _tienTrinhDieuTri.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                        _tienTrinhDieuTri.NgaySua = DateTime.Now;
                        _tienTrinhDieuTri.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                        await _unitOfWork.UpdateAsync<tbTienTrinhDieuTri, Guid>(_tienTrinhDieuTri);
                    }
                    ;

                    await CapNhat_TongChiPhi_LichDieuTri(_lichDieuTri.IdLichDieuTri);
                }
            });
        }
        #endregion
    }
}